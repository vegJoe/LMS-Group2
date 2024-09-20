using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Data
{
    public class LMSApiContext : IdentityDbContext<User, IdentityRole, string>
    {
        public LMSApiContext(DbContextOptions<LMSApiContext> options) : base(options)
        {
        }
        public DbSet<LMS.API.Models.Entities.Course> Course { get; set; } = default!;
        public DbSet<LMS.API.Models.Entities.Module> Module { get; set; } = default!;
        public DbSet<LMS.API.Models.Entities.Activity> Activity { get; set; } = default!;
    }
}