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
    public class ModulesController : ControllerBase
    {
        private readonly LMSApiContext _context;
        private readonly IMapper _mapper;

        public ModulesController(LMSApiContext context, IMapper mapper)
        {
            _context = context;
            this._mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all modules along with their associated courses and activities
        /// </summary>
        /// <returns>Returns a list of modules as ModuleDto</returns>
        // GET: api/Modules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModule(int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? filter = null)
        {
            var modules = await _context.Modules
                .Include(m => m.Course)
                .Include(m => m.Activites)
                .ToListAsync();

            if (modules == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Moduels not found",
                    Detail = "No Moduels were found in the system.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            var modulesDto = _mapper.Map<IEnumerable<ModuleDto>>(modules);
            return Ok(modulesDto);
            
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Invalid pageNumber or pageSize");
            }

            IQueryable<Module> query = _context.Modules.Include(c => c.Activities);

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
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(u => u.Name);
                        break;
                    case "courseid":
                        query = query.OrderBy(u => u.CourseId);
                        break;
                    default:
                        query = query.OrderBy(u => u.Id);
                        break;
                }
            }

            var totalModules = await query.CountAsync();

            if (totalModules == 0)
            {
                return NotFound("No users found.");
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

            //var modules = await _context.Modules
            //    .Include(m => m.Course)
            //    .Include(m => m.Activities)
            //    .ToListAsync();

            //var modulesDto = _mapper.Map<IEnumerable<ModuleDto>>(modules);
            //return Ok(modulesDto);
        }

        /// <summary>
        /// Retrieves a specific module by its ID
        /// </summary>
        /// <param name="id">The ID of the module to retrieve</param>
        /// <returns>Returns the module as a ModuleDto if found, or 404 if the module does not exist</returns>
        // GET: api/Modules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModuleDto>> GetModule(int id)
        {
            var @module = await _context.Modules
                .Where(m => m.Id == id)
                .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (@module == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Module not found",
                    Detail = $"Module with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(@module);
        }

        /// <summary>
        /// Updates an existing module
        /// </summary>
        /// <param name="id">The ID of the module to update</param>
        /// <param name="module">The updated module details</param>
        /// <returns>Returns 200 OK with a confirmation message if the update is successful, or 400 if the model state is invalid, or 404 if the module does not exist</returns>
        // PUT: api/Modules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModule(int id, CreateUpdateModuleDto @module)
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

            _mapper.Map(module, existingModule);

            await _context.SaveChangesAsync();

            return Ok($"Updated module id:{id}");
        }

        /// <summary>
        /// Creates a new module
        /// </summary>
        /// <param name="module">The details of the module to create</param>
        /// <returns>Returns the created module as a ModuleDto along with a 201 Created status if successful, or 400 if the creation fails</returns>
        // POST: api/Modules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ModuleDto>> PostModule(CreateUpdateModuleDto @module)
        {
            var newModule = _mapper.Map<Module>(@module);

            if (newModule != null)
            {
                _context.Modules.Add(newModule);
                await _context.SaveChangesAsync();
                return StatusCode(201, "New module was created"); //statuscode 201 for "Created".
            }
            return BadRequest(new ProblemDetails
            {
                Title = "Module Creation Failed",
                Detail = "The module could not be created due to an internal error.",
                Status = StatusCodes.Status400BadRequest,
                Instance = HttpContext.Request.Path
            });
        }


        /// <summary>
        /// Deletes a specific module by its ID
        /// </summary>
        /// <param name="id">The ID of the module to delete</param>
        /// <returns>Returns 200 OK with a confirmation message if the deletion is successful, or 404 if the module does not exist</returns>
        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var @module = await _context.Modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Module not found",
                    Detail = $"Module with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            _context.Modules.Remove(@module);
            await _context.SaveChangesAsync();

            return Ok($"Entry with id:{id} was deleted");
        }

        private bool ModuleExists(int id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }
    }
}
