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
                UserName = x.UserName ?? "",
                CourseId = x.CourseId ?? 0,
                Course = x.Course,
                RefreshToken = x.RefreshToken,
                RefreshTokenExpireTime = x.RefreshTokenExpireTime
            }).ToList();
        }

        // GET api/<UsersController>/5
        //[HttpGet("{id}")]
        //public UserDto Get(int id)
        //{
        //    return "value";
        //}
    }
}
