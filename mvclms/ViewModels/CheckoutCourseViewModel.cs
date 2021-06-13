using Microsoft.AspNetCore.Mvc;

namespace mvclms.ViewModels
{
    public class CheckoutCourseViewModel
    {
        [HiddenInput] public bool Sure { get; set; } = true;
        [HiddenInput] public int CourseId { get; set; }
    }
}