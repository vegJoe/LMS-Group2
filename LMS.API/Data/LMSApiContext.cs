using Microsoft.EntityFrameworkCore;
using LMS.API.Models.Entities;

namespace LMS.API.Data
{
    public class LMSApiContext : DbContext
    {
        public LMSApiContext(DbContextOptions<LMSApiContext> options)
    : base(options)
        {
        }

        public DbSet<Activity> Activity => Set<Activity>();
        public DbSet<ActivityType> ActivityType => Set<ActivityType>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<User> Users => Set<User>();
    }
}
