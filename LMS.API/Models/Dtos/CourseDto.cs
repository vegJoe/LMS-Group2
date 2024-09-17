using LMS.API.Models.Entities;

namespace LMS.API.Models.Dtos
{
    
    public record CourseDto(string Name,  string Description, DateTime StartDate, IEnumerable<Users> Users, IEnumerable<Modules> Modules);
    
}
