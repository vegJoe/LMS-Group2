namespace LMS.API.Models.Dtos
{
    public class ActivityTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ActivityDto> Activitys { get; set; }
    }
}
