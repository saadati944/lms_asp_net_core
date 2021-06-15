using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using mvclms.Models;

namespace mvclms.ViewModels
{
    public class CourseViewModel
    {
        [HiddenInput]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Price { get; set; }
    }
}