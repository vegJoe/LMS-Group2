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

        // GET: api/Modules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModule(int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? filter = null)
        {
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
                return NotFound();
            }

            return Ok(@module);
        }

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
                return NotFound();
            }

            _mapper.Map(module, existingModule);

            await _context.SaveChangesAsync();

            return Ok($"Updated module id:{id}");
        }

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
            return BadRequest("Could not create new module");
        }

        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var @module = await _context.Modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
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
