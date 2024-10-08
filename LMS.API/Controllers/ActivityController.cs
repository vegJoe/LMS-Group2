﻿using AutoMapper;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entitie = LMS.API.Models.Entities;

namespace LMS.API.Controllers
{
    [Authorize]
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
        [Authorize(Roles = "Teacher")]
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

        /// <summary>
        /// Returns Activity with choosen ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Activity/5
        [Authorize(Roles = "Teacher")]
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

        /// <summary>
        /// Creates Activity
        /// </summary>
        /// <param name="id">The activity ID to be updated</param>
        /// <param name="activity">The details of the activity to be created or updated</param>
        /// <returns></returns>
        // PUT: api/Activity/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Teacher")]
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

        /// <summary>
        /// Creates a new Activity
        /// </summary>
        /// <param name="activity">The details of the activity to be created</param>
        /// <returns>A status code 201 if the activity is successfully created, otherwise returns a bad request</returns>
        // POST: api/Activity
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Teacher")]
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

        /// <summary>
        /// Deletes an existing Activity
        /// </summary>
        /// <param name="id">The ID of the activity to be deleted</param>
        /// <returns>A status code 200 with a confirmation message if the deletion is successful, or a 404 if the activity is not found</returns>
        // DELETE: api/Activity/5
        [Authorize(Roles = "Teacher")]
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
