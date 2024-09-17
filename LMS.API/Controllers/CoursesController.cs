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

    // PUT: api/Courses/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, CourseToUpdateDto dto)
    {
        //ToDo Jimmy [HttpPut("{id}")]
        //if (dto.Rating < 0 || dto.Rating > 10)
        //{
        //    return BadRequest("Rating must be between 0 and 10.");
        //}

        //// Hämta den befintliga filmen från databasen
        //var existingMovie = await _context.Movie
        //    .Include(m => m.Director)
        //    .Include(m => m.Actors)
        //    .Include(m => m.Genres)
        //    .FirstOrDefaultAsync(m => m.Id == id);

        //if (existingMovie == null) return NotFound();


        //// Use AutoMapper to update the entity
        //_mapper.Map(dto, existingMovie);

        //try
        //{
        //    await _context.SaveChangesAsync();
        //}
        //catch (DbUpdateConcurrencyException)
        //{
        //    if (!MovieExists(id))
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        throw;
        //    }
        //}

        //return NoContent();
    }

    // DELETE: api/Courses/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        //var movie = await _context.Movie.FindAsync(id);
        //if (movie == null)
        //{
        //    return NotFound();
        //}

        //_context.Movie.Remove(movie);
        //await _context.SaveChangesAsync();

        //return NoContent();
    }

    
}
