using System.ComponentModel.DataAnnotations;

namespace LMS.API.Models.Dtos

{
    public class CourseDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Course name is required.")]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Course StartDate is required.")]
        public DateTime StartDate { get; set; }
        public IEnumerable<ModuleDto>? Modules { get; set; }

    }
}
