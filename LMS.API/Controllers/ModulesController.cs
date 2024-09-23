using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.API.Data;
using LMS.API.Models.Entities;
using LMS.API.Models.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;

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
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModule()
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

        // PUT: api/Modules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModule(int id, CreateUpdateModuleDto @module)
        {
            if(!ModelState.IsValid)
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

        // POST: api/Modules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ModuleDto>> PostModule(CreateUpdateModuleDto @module)
        {
            var newModule = _mapper.Map<Module>(@module);

            if(newModule != null)
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
