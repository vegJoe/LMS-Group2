using LMS.API.Models.Entities;

namespace LMS.API.Models.Dtos
{
    public class CoursesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public IEnumerable<UserDto> Users { get; set; }
        public IEnumerable<ModulesDto> Modules { get; set; }
    }
}
