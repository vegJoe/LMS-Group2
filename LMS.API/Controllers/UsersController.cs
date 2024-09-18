using LMS.API.Data;
using LMS.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LMSApiContext _context;
        public UsersController(LMSApiContext context)
        {
            _context = context;
        }
        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<UserDto> Get()
        {
            return _context.Users.Select(x => new UserDto
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email ?? "",
                UserId = x.Id,
                UserName = x.UserName ?? "",
                CourseId = x.CourseId ?? 0,
                Course = x.Course,
            }).ToList();
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
    }
}
