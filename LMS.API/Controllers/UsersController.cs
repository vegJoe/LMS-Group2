using AutoMapper;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using LMS.API.Models.Entities;
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

        /// <summary>
        /// Retrieves a list of all users
        /// </summary>
        /// <returns>Returns a list of users as UserDto, or 404 if no users are found</returns>
        // GET: api/<UsersController>
        /// <summary>
        /// Retrieves a paginated list of users.
        /// </summary>
        /// <param name="pageNumber">The number of the page to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <param name="sortBy">Optional sort field (name or email).</param>
        /// <param name="filter">Optional filter string to search users by name or email.</param>
        /// <returns>A paginated list of users.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get(int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? filter = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Invalid pageNumber or pageSize");
            }

            IQueryable<User> query = _context.Users;

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(u =>
                    (u.FirstName + " " + u.LastName).Contains(filter) ||
                    (!string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains(filter)));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(u => u.FirstName);
                        break;
                    case "email":
                        query = query.OrderBy(u => u.Email);
                        break;
                    default:
                        query = query.OrderBy(u => u.Id);
                        break;
                }
            }

            var totalUsers = await query.CountAsync();

            if (totalUsers == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Users not found",
                    Detail = "No Users were found in the system.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            var users = await query.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            var response = new
            {
                TotalUsers = totalUsers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Users = userDtos,
            };

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific user by their ID
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>Returns the user as a UserDto if found, or 404 if the user does not exist</returns>
        //GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = $"User with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        /// <summary>
        /// Deletes a specific user by their ID
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>Returns 204 No Content if the deletion is successful, or 404 if the user does not exist</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserDto>> Delete(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = $"User with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
