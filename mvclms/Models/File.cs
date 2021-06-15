namespace mvclms.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public int LectureId { get; set; }
        public Lecture Lecture { get; set; }
    }
}