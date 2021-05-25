namespace mvclms.Models
{
    public class StudentCourse
    {
        public string StudentId { get; set; }
        public Person Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}