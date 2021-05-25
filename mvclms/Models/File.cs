namespace mvclms.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Path { get; set; }
        
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}