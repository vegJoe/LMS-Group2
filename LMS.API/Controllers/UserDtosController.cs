using LMS.API.Data;
using LMS.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDtosController : ControllerBase
    {
        private readonly LMSApiContext _context;

        public UserDtosController(LMSApiContext context)
        {
            _context = context;
        }

        // GET: api/UserDtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUserDto()
        {
            return await _context.UserDto.ToListAsync();
        }

        // GET: api/UserDtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserDto(Guid id)
        {
            var userDto = await _context.UserDto.FindAsync(id);

            if (userDto == null)
            {
                return NotFound();
            }

            return userDto;
        }

        // PUT: api/UserDtos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDto(Guid id, UserDto userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest();
            }

            _context.Entry(userDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDtoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserDtos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUserDto(UserDto userDto)
        {
            _context.UserDto.Add(userDto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserDto", new { id = userDto.Id }, userDto);
        }

        // DELETE: api/UserDtos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDto(Guid id)
        {
            var userDto = await _context.UserDto.FindAsync(id);
            if (userDto == null)
            {
                return NotFound();
            }

            _context.UserDto.Remove(userDto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserDtoExists(Guid id)
        {
            return _context.UserDto.Any(e => e.Id == id);
        }
    }
}
