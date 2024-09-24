namespace LMS.API.Models.Dtos
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        //public CourseDto Course { get; set; }
        public IEnumerable<ActivityDto> Activities { get; set; }
    }
}
