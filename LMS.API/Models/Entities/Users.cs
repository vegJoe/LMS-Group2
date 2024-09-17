namespace LMS.API.Models.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
        public int CourseId { get; set; }
        public Courses Course { get; set; }
    }
}
