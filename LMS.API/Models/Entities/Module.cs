namespace LMS.API.Models.Entities
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        public Course Courses { get; set; }
        public ICollection<Activity> Activites { get; set; }
    }
}
