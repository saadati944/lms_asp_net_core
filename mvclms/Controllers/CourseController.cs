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

        [HttpGet]
        public IActionResult CreateCourse()
        {
            var user = _userManager.GetUser(User); 
            if (user is null || !user.IsTeacher)
                return Ok("You don't have permissions to create course !!!");
            return View();
        }

        [HttpPost]
        public IActionResult CreateCourse(ViewModels.CourseViewModel course)
        {
            var user = _userManager.GetUser(User); 
            if (user is null || !user.IsTeacher)
                return Ok("You don't have permissions to create course !!!");
            int id = _courseManager.CreateCourse(course, user);
            return RedirectToAction("ShowCourse", "Course", new {id = id});
        }
    }
}