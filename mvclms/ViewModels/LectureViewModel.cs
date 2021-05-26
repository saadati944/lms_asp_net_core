using Microsoft.AspNetCore.Http;

namespace mvclms.ViewModels
{
    public class LectureViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        
        public IFormFile Attachment { get; set; }
        public string AttachmentDesc { get; set; }
    }
}