
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
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var coursesDto = await _context.Courses
                .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (coursesDto == null) return NotFound();

            return Ok(coursesDto);
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

            if (courseDto == null) return NotFound();

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
        public async Task<ActionResult<CourseDto>> UpdateCourse(int id, CourseDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            _mapper.Map(dto, course);

            // Mark the entity as modified so that Entity Framework knows it needs to be updated in the database
            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Does the course exist in the database
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

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
        public async Task<ActionResult<CourseDto>> CreateCourse(CourseDto dto)
        {
            var course = _mapper.Map<Course>(dto);

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.Id }, _mapper.Map<CourseDto>(course));

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

            if (course == null) return NotFound();

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

            if (course == null) return NotFound($"Course with ID {id} not found.");

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(course.Users);

            return Ok(userDtos);
        }
    }
}
