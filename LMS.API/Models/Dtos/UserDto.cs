using LMS.API.Models.Entities;

namespace LMS.API.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public Course? Course { get; set; }
    }
}
