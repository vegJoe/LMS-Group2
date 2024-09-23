namespace LMS.API.Models.Dtos
{
    public class CourseDto
    {
        //ToDo Make Id to GUID, after make changes in mappingprofile
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public IEnumerable<UserDto>? UsersDto { get; set; }
        public IEnumerable<ModuleDto>? ModulesDto { get; set; }

    }
}
