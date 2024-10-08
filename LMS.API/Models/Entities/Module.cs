﻿using System.ComponentModel.DataAnnotations;

namespace LMS.API.Models.Entities
{
    public class Module
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        public ICollection<Activity> Activities { get; set; } = [];
    }
}
