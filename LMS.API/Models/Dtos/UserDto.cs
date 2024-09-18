using LMS.API.Models.Entities;

namespace LMS.API.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
