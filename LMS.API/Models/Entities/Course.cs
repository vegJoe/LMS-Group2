using System.ComponentModel.DataAnnotations;

namespace LMS.API.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public ICollection<User>? Users { get; set; }
        public ICollection<Module> Modules { get; set; } = [];
    }
}
