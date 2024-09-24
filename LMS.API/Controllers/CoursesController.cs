
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
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
        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses(int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? filter = null)
        {

          
                var coursesDto = await _context.Courses
                    .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                if (coursesDto == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Courses not found",
                        Detail = "No courses were found in the system.",
                        Status = 404,
                        Instance = HttpContext.Request.Path
                    });
                }

                return Ok(coursesDto);
            

            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Invalid pageNumber or pageSize");
            }

            IQueryable<Course> query = _context.Courses.Include(c => c.Modules).ThenInclude(m => m.Activities);

            // Apply filtering by course name
            if (!string.IsNullOrWhiteSpace(filter))
            {
                // Apply text filtering by name or date
                DateTime? parsedDate = DateTime.TryParse(filter, out DateTime result) ? result : (DateTime?)null;

                query = query.Where(u =>
                    u.Name.Contains(filter) ||
                    (parsedDate.HasValue && u.StartDate == parsedDate));
            }

            //Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(u => u.Name);
                        break;
                    case "startdate":
                        query = query.OrderBy(u => u.StartDate);
                        break;
                    default:
                        query = query.OrderBy(u => u.Id);
                        break;
                }
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
                PageNumbers = pageNumber,
                PageSize = pageSize,
                Courses = courseDtos
            };

            return Ok(response);


            //var coursesDto = await _context.Courses
            //    .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
            //    .ToListAsync();

            //if (coursesDto == null) return NotFound();

        }

        /// <summary>
        /// Retrieves a specific course by its ID
        /// </summary>
        /// <param name="id">The ID of the course to retrieve</param>
        /// <returns>Returns the course as a CourseDto if found, or 404 if the course does not exist</returns>
        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
           
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
        /// <param name="id">The ID of the course to update</param>
        /// <param name="dto">The updated course details</param>
        /// <returns>Returns the updated course as a CourseDto if successful, or 400 if the ID does not match, 404 if the course is not found</returns>
        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(int id, CreateUpdateCourseDto dto)
        {

            if (id != dto.Id)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "ID Mismatch",
                    Detail = $"The course ID in the URL ({id}) does not match the ID in the body ({dto.Id}).",
                    Status = 400,
                    Instance = HttpContext.Request.Path
                });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses
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

                _mapper.Map(dto, course);
                // Mark the entity as modified so that Entity Framework knows it needs to be updated in the database
                _context.Entry(course).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                var updatedCourseDto = _mapper.Map<CourseDto>(course);

                return Ok(updatedCourseDto);
           
        }

        /// <summary>
        /// Creates a new course
        /// </summary>
        /// <param name="dto">The details of the course to create</param>
        /// <returns>Returns the created course as a CourseDto, along with a 201 Created status and the location of the new course</returns>
        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
        /// <param name="id">The ID of the course to delete</param>
        /// <returns>Returns 204 No Content if the deletion is successful, or 404 if the course is not found</returns>
        // DELETE: api/Courses/5
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
        /// <param name="id">The ID of the course for which to retrieve students</param>
        /// <returns>Returns a list of students as UserDto if found, or 404 if the course does not exist</returns>
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
