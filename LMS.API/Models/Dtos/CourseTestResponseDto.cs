namespace LMS.API.Models.Dtos
{
    public class CourseTestResponseDto
    {
        public int TotalCourses { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<CourseDto> Courses { get; set; }
    }
}
