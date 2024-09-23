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

        // GET: api/Activity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetActivitys(int id)
        {
            var activityDto = await _context.Activities
                .Where(a => a.Id == id) 
                .Include(at => at.Type)
                .FirstOrDefaultAsync();

            if (activityDto == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Activity not found",
                    Detail = $"Activity with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
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
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Title = "Validation Error",
                    Detail = "One or more validation errors occurred. Please review the errors and try again.",
                    Status = 400,
                    Instance = HttpContext.Request.Path
                });
            }

            var existingActivity = await _context.Activities.FindAsync(id);
            if (existingActivity == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Activity not found",
                    Detail = $"Activity with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
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

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Title = "Validation error",
                    Detail = "One or more validation errors occurred.",
                    Status = 400,
                    Instance = HttpContext.Request.Path
                });
            }
            var newActivity = _mapper.Map<Entitie.Activity>(@activity);

            _context.Activities.Add(newActivity);
            await _context.SaveChangesAsync();

            return StatusCode(201, "New activity was created"); //statuscode 201 for "Created".
            
        }

        // DELETE: api/Activity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivityDto(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Activity not found",
                    Detail = $"Activity with ID {id} was not found.",
                    Status = 404,
                    Instance = HttpContext.Request.Path
                });
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return Ok($"Entry with id:{id} was deleted");
        }

        private bool ActivityDtoExists(int id)
        {
            return _context.Activities.Any(e => e.Id == id);
        }
    }
}
