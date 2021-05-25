using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace mvclms.Models
{
    public class Person : IdentityUser
    {
        [StringLength(150)]
        [Key]
        public override string Id { get; set; }
        
        [StringLength(100)]
        public override string UserName { get; set; }
        
        public List<Course> Courses { get; set; } = new();
        public List<StudentCourse> StudentCourses { get; set; } = new();

        [StringLength(100)]
        public string FirstName { get; set; }
        
        [StringLength(100)]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => FirstName + ", " + LastName;
    }
}