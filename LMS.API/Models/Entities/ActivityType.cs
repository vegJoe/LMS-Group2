using System.ComponentModel.DataAnnotations;

namespace LMS.API.Models.Entities
{
    public class ActivityType
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}
