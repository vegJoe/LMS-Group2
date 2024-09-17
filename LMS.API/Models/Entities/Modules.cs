namespace LMS.API.Models.Entities
{
    public class Modules
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        public Courses Course { get; set; }
        public ICollection<Activity> Activity { get; set; }
    }
}
