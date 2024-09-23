
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

            //return Ok(coursesDto);
        }

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
