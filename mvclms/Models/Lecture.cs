namespace mvclms.Models
{
    public class Lecture
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public File Attachment { get; set; }
        
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}