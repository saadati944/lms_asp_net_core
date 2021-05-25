using System;
using System.Collections.Generic;

namespace mvclms.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<File> Files { get; set; } = new();

        public string TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        
        public List<Student> Students { get; set; } = new();
        
    }
}