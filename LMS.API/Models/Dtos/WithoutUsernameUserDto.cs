namespace LMS.API.Models.Dtos
{
    public class WithoutUsernameUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }
}
