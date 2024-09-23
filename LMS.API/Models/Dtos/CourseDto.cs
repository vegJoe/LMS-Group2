﻿using LMS.API.Models.Entities;
using System;
using System.Collections.Generic;

namespace LMS.API.Models.Dtos
{
    public class CourseDto
    {
        //ToDo Make Id to GUID, after make changes in mappingprofile
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public IEnumerable<UserDto>? Users { get; set; }
        public IEnumerable<Module>? Module { get; set; }

    }

    public class CreateCourseDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
       
    }
}
