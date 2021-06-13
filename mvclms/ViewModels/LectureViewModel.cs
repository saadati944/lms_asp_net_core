using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvclms.ViewModels
{
    public class LectureViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public IFormFile Attachment { get; set; }
        public string AttachmentDesc { get; set; }
        
        public string Course { get; set; }

        public List<SelectListItem> Courses { get; set; }
    }
}