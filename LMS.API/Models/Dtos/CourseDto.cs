using LMS.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMS.API.Models.Dtos
{
    public class CourseDto
    {
        //ToDo Make Id to GUID, after make changes in mappingprofile
        public int Id { get; set; }
        [Required(ErrorMessage = "Course name is required.")]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Course StartDate is required.")]
        public DateTime StartDate { get; set; }
        public IEnumerable<UserDto>? Users { get; set; }
        public IEnumerable<Module>? Module { get; set; }

    }
}
