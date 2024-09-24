using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly LMSApiContext _context;
        private readonly IMapper _mapper;

        public CoursesController(LMSApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all available courses
        /// </summary>
        /// <returns>Returns a list of courses as CourseDto, or 404 if no courses are found</returns>
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses(int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? filter = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Invalid pageNumber or pageSize");
            }

            IQueryable<Course> query = _context.Courses.Include(c => c.Modules).ThenInclude(m => m.Activities);

            // Apply filtering by course name
            if (!string.IsNullOrWhiteSpace(filter))
            {
                DateTime? parsedDate = DateTime.TryParse(filter, out DateTime result) ? result : (DateTime?)null;

                query = query.Where(u =>
                    u.Name.Contains(filter) ||
                    (parsedDate.HasValue && u.StartDate == parsedDate));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "name" => query.OrderBy(u => u.Name),
                    "startdate" => query.OrderBy(u => u.StartDate),
                    _ => query.OrderBy(u => u.Id)
                };
            }

            var totalCourses = await query.CountAsync();

            if (totalCourses == 0)
            {
                return NotFound("No courses found");
            }

            var courses = await query.Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);

            var response = new
            {
                TotalCourses = totalCourses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Courses = courseDtos
            };

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific course by its ID, with role-based access control.
        /// </summary>
        /// <remarks>
        /// - Teachers can access any course.
        /// - Students can only access courses they are enrolled in.
        /// </remarks>
        /// <param name="id">The ID of the course to retrieve.</param>
        /// <returns>
        /// A CourseDto object if the user is authorized to access the course,
        /// or a NotFound or Forbid response based on access rules.
        /// </returns>
        /// <response code="200">Returns the course details if the user has access.</response>
        /// <response code="403">Forbidden if the user does not have access to the course.</response>
        /// <response code="404">Not found if the course does not exist.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Student")
            {
                var userCourseId = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.CourseId)
                    .FirstOrDefaultAsync();

                if (userCourseId != id)
                {
                    return Forbid(); // Deny access for students not enrolled
                }
            }

            var courseDto = await _context.Courses
                    .Where(c => c.Id == id)
                    .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();


            if (courseDto == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Course not found",
                    Detail = $"Course with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(courseDto);
        }

        /// <summary>
        /// Updates an existing course
        /// </summary>
        /// 
        [Authorize(Roles = "Teacher")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(int id, CreateUpdateCourseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Course not found",
                    Detail = $"Course with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            _mapper.Map(dto, course);
            _context.Entry(course).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            var updatedCourseDto = _mapper.Map<CourseDto>(course);

            return Ok(updatedCourseDto);
        }

        /// <summary>
        /// Creates a new course
        /// </summary>
        /// 
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse(CreateUpdateCourseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Title = "Validation error",
                    Detail = "One or more validation errors occurred when trying to create the course.",
                    Status = 400,
                    Instance = HttpContext.Request.Path
                });
            }

            var course = _mapper.Map<Course>(dto);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, new ProblemDetails
            {
                Title = "Resource created",
                Detail = $"Course with ID {course.Id} has been created successfully.",
                Status = 201,
                Instance = HttpContext.Request.Path
            });
        }

        /// <summary>
        /// Deletes a course by its ID
        /// </summary>
        /// 
        [Authorize(Roles = "Teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Course not found",
                    Detail = $"Course with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }

        /// <summary>
        /// Retrieves a list of students enrolled in a specific course
        /// </summary>
        [HttpGet("{id}/students")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetStudentsForCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Course not found",
                    Detail = $"Course with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(course.Users);
            return Ok(userDtos);
        }
    }
}
