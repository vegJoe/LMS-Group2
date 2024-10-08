﻿using LMS.API.Models.Entities;

namespace LMS.API.Models.Dtos
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Details { get; set; }
        public int TypeId { get; set; }
        public ActivityType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ModuleId { get; set; }
    }
}
