using System.Reflection;

namespace LMS.API.Models.Entities
{
    public class Courses
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public ICollection<Users> Users { get; set; }
        public ICollection<Modules> Modules { get; set; }
    }
}
