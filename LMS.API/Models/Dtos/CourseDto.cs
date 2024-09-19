namespace LMS.API.Models.Dtos
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public IEnumerable<UserDto> Users { get; set; }
        public IEnumerable<ModuleDto> Modules { get; set; }
    }
}
