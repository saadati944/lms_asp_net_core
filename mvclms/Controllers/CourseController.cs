using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvclms.Services;
using mvclms.ViewModels;

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

        [HttpGet]
        public IActionResult CreateLecture()
        {
            var user = _userManager.GetUser(User); 
            if (user is null || !user.IsTeacher)
                return Ok("You don't have permissions to create course !!!");
            ViewModels.LectureViewModel lvm = new LectureViewModel();
            lvm.Courses = _courseManager.GetTeacherCourses(user.Id)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            return View(lvm);
        }
        
        [HttpPost]
        public IActionResult CreateLecture(ViewModels.LectureViewModel lecture)
        {
            var user = _userManager.GetUser(User); 
            if (user is null || !user.IsTeacher)
                return Ok("You don't have permissions to create course !!!");
            int id = _courseManager.CreateLecture(lecture);
            return RedirectToAction("ShowLecture", "Course", new {id = id});
        }

        [HttpGet]
        public IActionResult CheckoutCourse(int id)
        {
            var user = _userManager.GetUser(User); 
            if (user is null || user.IsTeacher)
                return Ok("You don't have permissions to create course !!!");
            ViewBag.model = _courseManager.GetCourse(id);
            return View(new CheckoutCourseViewModel{ CourseId = id });
        }

        [HttpPost]
        public IActionResult CheckoutCourse(CheckoutCourseViewModel co)
        {
            if (!co.Sure)
                return Ok("checkout failed !!!");
         
            var user = _userManager.GetUser(User); 
            if (user is null || user.IsTeacher)
                return Ok("You don't have permissions to create course !!!");   
            
            _courseManager.CheckoutCourse(co.CourseId, user);
            
            return RedirectToAction("StudentCourses", "Course");
        }

        public IActionResult StudentCourses()
        {
            var user = _userManager.GetUser(User); 
            if (user is null || user.IsTeacher)
                return Ok("You don't have permissions to create course !!!");
            return Ok(_courseManager.GetStudentCourses(user.Id));
        }
    }
}