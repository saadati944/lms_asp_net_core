using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            ViewBag.navbar = new List<NavbarButton>();
            
        }

        private void addUserNavbar()
        {
            _userManager.AddNavigationBarButtons(ViewBag, User);
        }

        public IActionResult Courses(int? skip, int? count)
        {
            addUserNavbar();
            return View(_courseManager.GetCourses(skip ?? 0, count ?? 15));
        }
        
        public IActionResult ShowCourse(int id)
        {
            addUserNavbar();
            if (_userManager.GetUser(User) is null)
                ViewBag.IsCheckedOut = false;
            else
            {
                ViewBag.IsCheckedOut = _courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id, id,
                    _userManager.GetUser(User).IsTeacher);
                if (!ViewBag.IsCheckedOut && _userManager.isStudent)
                {
                    ViewBag.navbar.Add
                    (new NavbarButton
                    {
                        Title = "CheckOut",
                        Controller = "Course",
                        Action = "CheckoutCourse",
                        Id = id.ToString()
                    });
                }
                else if(ViewBag.IsCheckedOut && _userManager.isTeacher)
                {
                    ViewBag.navbar.Add
                    (new NavbarButton
                    {
                        Title = "Add Lecture",
                        Controller = "Course",
                        Action = "CreateLecture",
                        Id = id.ToString()
                    });
                }
            }
            
            return View(_courseManager.GetCourse(id));
        }

        public IActionResult ShowLecture(int id)
        {
            addUserNavbar();
            var lecture = _courseManager.GetLecture(id);
            if (_userManager.GetUser(User) is null || !_courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id,
                lecture.CourseId,
                _userManager.GetUser(User).IsTeacher))
                return Ok("you don't have permissions to show this lecture !!!");

            if (_userManager.isTeacher)
                ViewBag.navbar.Add
                    (new NavbarButton
                    {
                        Title = "Edit",
                        Controller = "Course",
                        Action = "UpdateLecture",
                        Id = id.ToString(),
                    });
            return View(lecture);
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            addUserNavbar();
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
        public IActionResult CreateLecture(int id)
        {
            addUserNavbar();
            _userManager.GetUser(User);
            if (!_userManager.isTeacher)
                return Ok("You don't have permissions to create new lecture !!!");
            
            LectureViewModel lvm = new LectureViewModel();
            lvm.CourseId = id;
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
        public IActionResult UpdateLecture(int id)
        {
            addUserNavbar();
            var lecture = _courseManager.GetLecture(id);
            if (_userManager.GetUser(User) is null || !_courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id,
                lecture.CourseId,
                true))
                return Ok("you don't have permissions to update this lecture !!!");

            UpdateLectureViewModel ulvm = new UpdateLectureViewModel
            {
                Title = lecture.Title,
                Content = lecture.Content,
                CourseId = lecture.CourseId,
                LectureId = id,
                AttachmentDesc = lecture.Attachment.Description
            };
            
            return View(ulvm);
        }

        [HttpPost]
        public IActionResult UpdateLecture(UpdateLectureViewModel lecture)
        {
            var lc = _courseManager.GetLecture(lecture.LectureId);
            if (_userManager.GetUser(User) is null || !_courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id,
                lecture.CourseId,
                true))
                return Ok("you don't have permissions to update this lecture !!!");
            
            _courseManager.UpdateLecture(lecture);
            
            return RedirectToAction("ShowLecture", "Course", new {id = lecture.LectureId});
        }

        [HttpGet]
        public IActionResult CheckoutCourse(int id)
        {
            addUserNavbar();
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
            addUserNavbar();
            _userManager.GetUser(User);
            if (!_userManager.isStudent)
                return Ok("You don't have permissions to visit this page !!!");
            return View(_courseManager.GetStudentCourses(_userManager.GetUser(User).Id));
        }
        
        public IActionResult TeacherCourses()
        {
            addUserNavbar();
            _userManager.GetUser(User);
            if (!_userManager.isTeacher)
                return Ok("You don't have permissions to visit this page !!!");
            return View(_courseManager.GetTeacherCourses(_userManager.GetUser(User).Id));
        }
    }
}