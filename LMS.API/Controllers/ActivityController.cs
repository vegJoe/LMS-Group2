using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using AutoMapper;
using System.Diagnostics;
using Entitie = LMS.API.Models.Entities;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly LMSApiContext _context;
        private readonly IMapper _mapper;

        public ActivityController(LMSApiContext context, IMapper mapper)
        {
            _context = context;
            this._mapper = mapper;
        }

        // GET: api/Activity
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivity()
        //{
        //    var activites
        //}

        // GET: api/Activity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetActivitys(int id)
        {
            var activityDto = await _context.Activities
                .Where(a => a.ModuleId == id)
                .FirstOrDefaultAsync();

            if (activityDto == null)
            {
                return NotFound();
            }

            return Ok(activityDto);
        }

        // PUT: api/Activity/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivityDto(int id, CreateUpdateActivityDto @activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingActivity = await _context.Modules.FindAsync(id);
            if (existingActivity == null)
            {
                return NotFound();
            }

            _mapper.Map(activity, existingActivity);

            await _context.SaveChangesAsync();

            return Ok($"Updated activity id:{id}");
        }

        // POST: api/Activity
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ActivityDto>> PostActivity(CreateUpdateActivityDto @activity)
        {
            var newActivity = _mapper.Map<Entitie.Activity>(@activity);

            if (newActivity != null)
            {
                _context.Activities.Add(newActivity);
                await _context.SaveChangesAsync();
                return StatusCode(201, "New activity was created"); //statuscode 201 for "Created".
            }
            return BadRequest("Could not create new activity");
        }

        // DELETE: api/Activity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivityDto(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActivityDtoExists(int id)
        {
            return _context.Activities.Any(e => e.Id == id);
        }
    }
}
