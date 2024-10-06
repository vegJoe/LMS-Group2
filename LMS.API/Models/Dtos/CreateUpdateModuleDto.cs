namespace LMS.API.Models.Dtos
{
    public class CreateUpdateModuleDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
