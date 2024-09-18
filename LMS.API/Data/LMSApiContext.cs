using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Data
{
    public class LMSApiContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public LMSApiContext(DbContextOptions<LMSApiContext> options) : base(options)
        {
        }
         public DbSet<LMS.API.Models.Entities.Course> Course { get; set; } = default!;
        //public DbSet<LMS.API.Models.Dtos.CourseDto> CourseDto { get; set; } = default!;
    }
}