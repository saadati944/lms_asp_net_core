using System.Collections.Generic;

namespace mvclms.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Course> courses { get; set; } = new();
    }
}