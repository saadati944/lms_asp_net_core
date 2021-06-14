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

        public IActionResult Courses(int? skip, int? count)
        {
            return View(_courseManager.GetCourses(skip ?? 0, count ?? 15));
        }
        
        public IActionResult ShowCourse(int id)
        {
            if (_userManager.GetUser(User) is null)
                ViewBag.IsCheckedOut = false;
            else
            {
                ViewBag.IsCheckedOut = _courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id, id,
                    _userManager.GetUser(User).IsTeacher);
                if (!ViewBag.IsCheckedOut)
                    ViewBag.navbar = new []{
                        new NavbarButton
                    {
                        Title = "CheckOut",
                        Controller = "Course",
                        Action = "CheckoutCourse",
                        Id = id.ToString()
                    } };
            }
            
            return View(_courseManager.GetCourse(id));
        }

        public IActionResult ShowLecture(int id)
        {
            var lecture = _courseManager.GetLecture(id);
            if (_userManager.GetUser(User) is null || !_courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id,
                lecture.CourseId,
                _userManager.GetUser(User).IsTeacher))
                return Ok("you don't have permissions to show this lecture !!!");
            return View(lecture);
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            _userManager.GetUser(User);
            if (!_userManager.isTeacher)
                return Ok("You don't have permissions to create course !!!");
            return View();
        }

        [HttpPost]
        public IActionResult CreateCourse(ViewModels.CourseViewModel course)
        {
            _userManager.GetUser(User);
            if (!_userManager.isTeacher)
                return Ok("You don't have permissions to create course !!!");
            int id = _courseManager.CreateCourse(course, _userManager.GetUser(User));
            return RedirectToAction("ShowCourse", "Course", new {id = id});
        }

        [HttpGet]
        public IActionResult CreateLecture()
        {
            _userManager.GetUser(User);
            if (!_userManager.isTeacher)
                return Ok("You don't have permissions to create lecture !!!");
            ViewModels.LectureViewModel lvm = new LectureViewModel();
            lvm.Courses = _courseManager.GetTeacherCourses(_userManager.GetUser(User).Id)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            if (lvm.Courses.Count == 0)
                return Ok("Create a course first !!!");
            return View(lvm);
        }

        [HttpPost]
        public IActionResult CreateLecture(ViewModels.LectureViewModel lecture)
        {
            _userManager.GetUser(User);
            if (!_userManager.isTeacher)
                return Ok("You don't have permissions to create lecture !!!");
            int id = _courseManager.CreateLecture(lecture);
            return RedirectToAction("ShowLecture", "Course", new {id = id});
        }

        [HttpGet]
        public IActionResult CheckoutCourse(int id)
        {
            _userManager.GetUser(User);
            if (!_userManager.isStudent)
                return Ok("You don't have permissions to checkout course !!!");
            ViewBag.model = _courseManager.GetCourse(id);
            return View(new CheckoutCourseViewModel {CourseId = id});
        }

        [HttpPost]
        public IActionResult CheckoutCourse(CheckoutCourseViewModel co)
        {
            _userManager.GetUser(User);
            if (!co.Sure)
                return Ok("checkout failed !!!");

            if (!_userManager.isStudent)
                return Ok("You don't have permissions to checkout course page !!!");

            //TODO: use a payment method to pay the price

            _courseManager.CheckoutCourse(co.CourseId, _userManager.GetUser(User));

            return RedirectToAction("StudentCourses", "Course");
        }

        public IActionResult StudentCourses()
        {
            _userManager.GetUser(User);
            if (!_userManager.isStudent)
                return Ok("You don't have permissions to visit this page !!!");
            return View(_courseManager.GetStudentCourses(_userManager.GetUser(User).Id));
        }
    }
}