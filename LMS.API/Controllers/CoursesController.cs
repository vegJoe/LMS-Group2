
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

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
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
            
        }

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

        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(int id, CourseDto dto)
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

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse(CourseDto dto)
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
