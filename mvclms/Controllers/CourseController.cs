using Microsoft.AspNetCore.Mvc;
using mvclms.Services;

namespace mvclms.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseManager _courseManager;
        private readonly MyUserManager _userManager;

        public CourseController(CourseManager courseManager, MyUserManager userManager)
        {
            _courseManager = courseManager;
            _userManager = userManager;
        }
        
        public IActionResult ShowCourse(int id)
        {
            return View(_courseManager.GetCourse(id));
        }

        public IActionResult ShowLecture(int id)
        {
            return View(_courseManager.GetLecture(id));
        }
    }
}