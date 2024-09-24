namespace LMS.API.Models.Dtos
{
    public class CreateUpdateCourseDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
    }
}
