﻿using LMS.API.Models.Entities;

namespace LMS.API.Models.Dtos
{
    public class ModulesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public IEnumerable<ActivityDto> Activity { get; set; }
    }
}