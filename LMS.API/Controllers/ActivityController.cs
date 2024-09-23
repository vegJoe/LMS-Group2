using AutoMapper;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            _mapper = mapper;
        }

        [HttpGet]
        // Get all activites
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities(int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? filter = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Invalid pageNumber or pageSize");
            }

            IQueryable<LMS.API.Models.Entities.Activity> query = _context.Activities;

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
                    case "startdate":
                        query = query.OrderBy(u => u.StartDate);
                        break;
                    default:
                        query = query.OrderBy(u => u.Id);
                        break;
                }
            }

            var totalActivites = await query.CountAsync();

            if (totalActivites == 0)
            {
                return NotFound("No users found.");
            }

            var activities = await query.Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            var activityDtos = _mapper.Map<IEnumerable<ActivityDto>>(activities);

            var response = new
            {
                TotalActivites = totalActivites,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Activities = activityDtos,
            };

            return Ok(response);
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

            var existingActivity = await _context.Activities.FindAsync(id);
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

            return Ok($"Entry with id:{id} was deleted");
        }

        private bool ActivityDtoExists(int id)
        {
            return _context.Activities.Any(e => e.Id == id);
        }
    }
}
