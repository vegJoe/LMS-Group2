using AutoMapper;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LMSApiContext _context;
        private readonly IMapper _mapper;
        public UsersController(LMSApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get()
        {
            var users = await _context.Users.ToListAsync();

            if (users.Count == 0)
            {
                return NotFound("No users found.");
            }

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(userDtos);
        }

        //GET api/<UsersController>/5
        [HttpGet("{id}")]
        public UserDto Get(string id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserId = user.Id,
                UserName = user.UserName ?? "",
                CourseId = user.CourseId ?? 0,
                Course = user.Course,
            };
        }

        [HttpDelete("{id}")]
        public ActionResult<UserDto> Delete(string id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            var userDto = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserId = user.Id,
                UserName = user.UserName ?? "",
                CourseId = user.CourseId ?? 0,
                Course = user.Course,
            };

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return Ok(userDto);
            }

            return BadRequest();
        }
    }
}
