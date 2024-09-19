using LMS.API.Models.Entities;
using System;
using System.Collections.Generic;

namespace LMS.API.Models.Dtos
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public IEnumerable<UserDto> Users { get; set; }
        public IEnumerable<Module> Module { get; set; }   
        
    }
}
