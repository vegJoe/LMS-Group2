using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly LMSApiContext _context;
        private readonly IMapper _mapper;

        public ModulesController(LMSApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all modules along with their associated courses and activities
        /// </summary>
        // GET: api/Modules
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules(int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? filter = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Invalid pageNumber or pageSize");
            }

            IQueryable<Module> query = _context.Modules
                .Include(m => m.Course)
                .Include(m => m.Activities);

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(u =>
                    u.Name.Contains(filter) ||
                    (!string.IsNullOrWhiteSpace(u.Description) && u.Description.Contains(filter)));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "name" => query.OrderBy(u => u.Name),
                    "courseid" => query.OrderBy(u => u.CourseId),
                    _ => query.OrderBy(u => u.Id),
                };
            }

            var totalModules = await query.CountAsync();

            if (totalModules == 0)
            {
                return NotFound("No modules found.");
            }

            var modules = await query.Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            var moduleDtos = _mapper.Map<IEnumerable<ModuleDto>>(modules);

            var response = new
            {
                TotalModules = totalModules,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Modules = moduleDtos,
            };

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific module by its ID, with role-based access control.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to retrieve module details based on their role:
        /// - Teachers can access any module regardless of course ID.
        /// - Students can only access modules for courses they are enrolled in.
        /// </remarks>
        /// <param name="id">The ID of the module to retrieve.</param>
        /// <returns>A ModuleDto object if found, or a NotFound or Forbid response based on access rules.</returns>
        /// <response code="200">Returns the module details if the user has access.</response>
        /// <response code="403">Forbidden if the user is not authorized to access the module.</response>
        /// <response code="404">Not found if the module does not exist.</response>
        // GET: api/Modules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModuleDto>> GetModule(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Fetch the module to get the CourseID
            var module = await _context.Modules
                    .Where(m => m.Id == id)
                    .Select(m => new { m.Id, m.CourseId })
                    .FirstOrDefaultAsync();

            if (module == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Module not found",
                    Detail = $"Module with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            if (userRole == "Teacher")
            {
                var moduleDto = await _context.Modules
                    .Where(m => m.Id == id)
                    .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                return Ok(moduleDto);
            }

            // Fetch the users CourseID
            var userCourseId = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.CourseId)
                .FirstOrDefaultAsync();

            if (userCourseId != module.CourseId)
            {
                return Forbid();
            }

            var moduleDtoResponse = await _context.Modules
                .Where(m => m.Id == id)
                .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return Ok(moduleDtoResponse);
        }

        /// <summary>
        /// Retrieves resources restricted to users with the Teacher role.
        /// </summary>
        /// <remarks>
        /// This endpoint can only be accessed by users who have the "Teacher" role.
        /// Unauthorized access will result in a 403 Forbidden response.
        /// </remarks>
        /// <response code="200">Returns the requested resource if the user is authorized.</response>
        /// <response code="403">Forbidden if the user does not have the Teacher role.</response>
        // PUT: api/Modules/5
        [Authorize(Roles = "Teacher")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModule(int id, CreateUpdateModuleDto moduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingModule = await _context.Modules.FindAsync(id);

            if (existingModule == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Module not found",
                    Detail = $"Module with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            _mapper.Map(moduleDto, existingModule);
            await _context.SaveChangesAsync();

            return Ok($"Updated module id:{id}");
        }

        /// <summary>
        /// Retrieves resources restricted to users with the Teacher role.
        /// </summary>
        /// <remarks>
        /// This endpoint can only be accessed by users who have the "Teacher" role.
        /// Unauthorized access will result in a 403 Forbidden response.
        /// </remarks>
        /// <response code="200">Returns the requested resource if the user is authorized.</response>
        /// <response code="403">Forbidden if the user does not have the Teacher role.</response>
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<ActionResult<ModuleDto>> PostModule(CreateUpdateModuleDto moduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Title = "Validation error",
                    Detail = "One or more validation errors occurred when trying to create the module.",
                    Status = 400,
                    Instance = HttpContext.Request.Path
                });
            }

            var newModule = _mapper.Map<Module>(moduleDto);
            _context.Modules.Add(newModule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetModule), new { id = newModule.Id }, newModule);
        }

        /// <summary>
        /// Retrieves resources restricted to users with the Teacher role.
        /// </summary>
        /// <remarks>
        /// This endpoint can only be accessed by users who have the "Teacher" role.
        /// Unauthorized access will result in a 403 Forbidden response.
        /// </remarks>
        /// <response code="200">Returns the requested resource if the user is authorized.</response>
        /// <response code="403">Forbidden if the user does not have the Teacher role.</response>
        // DELETE: api/Modules/5
        [Authorize(Roles = "Teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Module not found",
                    Detail = $"Module with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
