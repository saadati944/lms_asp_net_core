using System;
using System.Collections.Generic;

namespace mvclms.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Price { get; set; }
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<Lecture> Lectures { get; set; } = new();

        public string TeacherId { get; set; }
        public Person Teacher { get; set; }
        
        public List<StudentCourse> StudentCourses { get; set; } = new();
        
    }
}