
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly LMSApiContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(LMSApiContext context, IMapper mapper, ILogger<CoursesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            try
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

                return Ok(new ProblemDetails
                {
                    Title = "Request successful",
                    Detail = "Courses fetched successfully.",
                    Status = 200,
                    Instance = HttpContext.Request.Path
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while fetching courses.",
                    Status = 500,
                    Instance = HttpContext.Request.Path
                });
            }
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching course with ID {id}");
                return StatusCode(500, new { message = "An error occurred while fetching the course." });
            }
        }

        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(int id, CourseDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "Course ID does not match." });
            try
            {
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
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error when updating course with ID {id}");

                if (!CourseExists(id))
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Course not found",
                        Detail = "The course no longer exists.",
                        Status = 404,
                        Instance = HttpContext.Request.Path
                    });
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating course with ID {id}");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = $"An error occurred while updating the course with ID {id}.",
                    Status = 500,
                    Instance = HttpContext.Request.Path
                });
            }

        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse(CourseDto dto)
        {
            try
            {
                var course = _mapper.Map<Course>(dto);

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCourse", new { id = course.Id }, new ProblemDetails
                {
                    Title = "Resource created",
                    Detail = $"Course with ID {course.Id} has been created successfully.",
                    Status = 201,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while creating the course.",
                    Status = 500,
                    Instance = HttpContext.Request.Path
                });
            }



        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting course with ID {id}");
                return StatusCode(500, new { message = "An error occurred while deleting the course." });
            }
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }

        [HttpGet("{id}/students")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetStudentsForCourse(int id)
        {
            try
            {
                var course = await _context.Courses
                    .Include(c => c.Users)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (course == null) return NotFound(new { message = $"Course with ID {id} not found." });

                var userDtos = _mapper.Map<IEnumerable<UserDto>>(course.Users);

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching students for course with ID {id}");
                return StatusCode(500, new { message = "An error occurred while fetching students." });
            }
        }
    }
}
