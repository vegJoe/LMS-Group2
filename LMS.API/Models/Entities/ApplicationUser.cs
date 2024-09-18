using Microsoft.AspNetCore.Identity;

namespace LMS.API.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public Course Course { get; set; }
}
