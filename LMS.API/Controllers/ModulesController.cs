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

        /// <summary>
        /// Retrieves a list of all modules along with their associated courses and activities
        /// </summary>
        /// <returns>Returns a list of modules as ModuleDto</returns>
        // GET: api/Modules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModule()
        {
            var modules = await _context.Modules
                .Include(m => m.Course)
                .Include(m => m.Activites)
                .ToListAsync();
                
            var modulesDto = _mapper.Map<IEnumerable<ModuleDto>>(modules);
            return Ok(modulesDto);
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
                return NotFound();
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
            if(!ModelState.IsValid)
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

            if(newModule != null)
            {
                _context.Modules.Add(newModule);
                await _context.SaveChangesAsync();
                return StatusCode(201, "New module was created"); //statuscode 201 for "Created".
            }
            return BadRequest("Could not create new module");
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
