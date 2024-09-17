using LMS.API.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        // ToDo database?
    }

    // GET: api/Courses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        //ToDo Jimmy api/Courses
        //var actorsDto = await _context.Actor.ProjectTo<ActorDto>(_mapper.ConfigurationProvider).ToListAsync();

        //if (actorsDto == null) return NotFound();

        //return Ok(actorsDto);

    }

    // GET: api/Courses/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(int id)
    {
        //ToDo Jimmy api/Courses/{id}
        //var movie = await _context.Movie.FindAsync(id);

        //if (movie == null)
        //{
        //    return NotFound();
        //}

        //// Mappa från Movie till MovieDto
        //var movieDto = _mapper.Map<MovieDto>(movie);

        //return Ok(movieDto);
    }
}
