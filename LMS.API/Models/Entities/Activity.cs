using System.ComponentModel.DataAnnotations;

namespace LMS.API.Models.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Details { get; set; }
        [Required]
        public int TypeId { get; set; }
        public ActivityType Type { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int ModuleId { get; set; }
        public Module Modules { get; set; }
    }
}
