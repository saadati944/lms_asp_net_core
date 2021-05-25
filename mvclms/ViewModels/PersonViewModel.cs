using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvclms.Models;

namespace mvclms.ViewModels
{
    public abstract class PersonViewModel
    {
        [Required]
        [DisplayName("User Name")]   
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(100)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(100)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        
        public string PersonMode { get; set; }

        public List<SelectListItem> PersonModes { get; set; } = new List<SelectListItem>
            {new SelectListItem("Teacher", "Teacher"), new SelectListItem("Student", "Student")};
    }
}