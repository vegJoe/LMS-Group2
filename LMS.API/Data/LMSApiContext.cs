using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LMS.API.Models.Dtos;

namespace LMS.API.Data
{
    public class LMSApiContext : IdentityDbContext<User, IdentityRole, string>
    {
        public LMSApiContext(DbContextOptions<LMSApiContext> options) : base(options)
        {
        }
        public DbSet<LMS.API.Models.Dtos.UserDto> UserDto { get; set; } = default!;
    }
}