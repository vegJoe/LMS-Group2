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
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModule()
        {
            var modules = await _context.Module.Include(m => m.Course)
                .Include(m => m.Activites)
                .ToListAsync();
            
            if(modules == null)
            {
                return NotFound();
            }

            var modulesDto = _mapper.Map<IEnumerable<ModuleDto>>(modules);
            return Ok(modulesDto);
        }

        // GET: api/Modules/5
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Module>> GetModule(int id)
        {
            var module = await _context.Module.Include(m => m.Course)
                .Include(m => m.Activites)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (module == null)
            {
                return NotFound();
            }

            var moudleDto = _mapper.Map<ModuleDto>(module);

            return Ok(moudleDto);
        }

        // PUT: api/Modules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> PutModule(int id, Module @module)
        {
            if (id != @module.Id)
            {
                return BadRequest();
            }

            _context.Entry(@module).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(id))
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

        // POST: api/Modules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Module>> PostModule(ModuleDto module)
        {
            var newModule = _mapper.Map<Module>(module);

            if(newModule != null)
            {
                _context.Module.Add(newModule);
                await _context.SaveChangesAsync();
                return CreatedAtAction("Module", new { id  = newModule.Id }, module);
            }

            return BadRequest("Was unable to create new Module");
        }

        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var @module = await _context.Module.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }

            _context.Module.Remove(@module);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModuleExists(int id)
        {
            return _context.Module.Any(e => e.Id == id);
        }
    }
}
