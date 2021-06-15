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

        private void AddNavigation(string title, string controller, string action, int id, string area = "")
        {
            AddNavigation(title, controller, action, id.ToString(), area);
        }

        private void AddNavigation(string title, string controller, string action, string id = null, string area = "")
        {
            ViewBag.navbar.Add
            (new NavbarButton
            {
                Title = title,
                Controller = controller,
                Action = action,
                Id = id,
                Area = area
            });
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
                    AddNavigation("CheckOut", "Course", "Course", id);
                else if (ViewBag.IsCheckedOut && _userManager.isTeacher)
                {
                    AddNavigation("Add Lecture", "Course", "CreateLecture", id);
                    AddNavigation("Edit", "Course", "UpdateCourse", id);
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
            {
                AddNavigation("Edit", "Course", "UpdateLecture", id);
                AddNavigation("Remove", "Course", "RemoveLecture", id);
            }

            AddNavigation("Back", "Course", "ShowCourse", lecture.CourseId);
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

            AddNavigation("Back", "Course", "ShowCourse", id);

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
            AddNavigation("Back", "Course", "ShowLecture", id);
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
        public IActionResult RemoveLecture(int id)
        {
            addUserNavbar();
            var lecture = _courseManager.GetLecture(id);
            if (_userManager.GetUser(User) is null || !_courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id,
                lecture.CourseId,
                true))
                return Ok("you don't have permissions to update this lecture !!!");
            
            AddNavigation("Back", "Course", "ShowLecture", id);
            
            ViewBag.lecture = lecture;
            return View(new ConfirmOperationViewModel{Id = id});
        }
        
        [HttpPost]
        public IActionResult RemoveLecture(ConfirmOperationViewModel confirm)
        {
            // int courseid = _courseManager.GetLectureCourse(confirm.Id);
            // if (_userManager.GetUser(User) is null || !_courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id,
            //     courseid,
            //     true))
            //     return Ok("you don't have permissions to update this lecture !!!");
            // _courseManager.RemoveLecture(confirm.Id);
            // return RedirectToAction("ShowCourse", "Course", new {id = courseid});
            var lecture = _courseManager.GetLecture(confirm.Id);
            if (_userManager.GetUser(User) is null || !_courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id,
                lecture.CourseId,
                true))
                return Ok("you don't have permissions to update this lecture !!!");
            _courseManager.RemoveLecture(lecture);
            return RedirectToAction("ShowCourse", "Course", new {id = lecture.CourseId});
        }

        [HttpGet]
        public IActionResult UpdateCourse(int id)
        {
            addUserNavbar();
            if (_userManager.GetUser(User) is null || !_userManager.isTeacher)
                return Ok("you don't have permissions to update this course !!!");

            var course = _courseManager.GetTeacherCourses(_userManager.GetUser(User).Id)
                .FirstOrDefault(x => x.Id == id);

            if (course is null)
                return Ok("you don't have permissions to update this course !!!");

            CourseViewModel cvm = new CourseViewModel
            {
                Id = id,
                Name = course.Name,
                Category = course.Category.Name,
                Description = course.Description,
                Price = course.Price,
                StartDate = course.StartDate,
                EndDate = course.EndDate
            };

            AddNavigation("Back", "Course", "ShowCourse", id);
            return View(cvm);
        }

        [HttpPost]
        public IActionResult UpdateCourse(CourseViewModel cvm)
        {
            if (_userManager.GetUser(User) is null || !_userManager.isTeacher ||
                !_courseManager.IsCourseCheckedOut(_userManager.GetUser(User).Id, cvm.Id, true))
                return Ok("you don't have permissions to update this course !!!");

            _courseManager.UpdateCourse(cvm);

            return RedirectToAction("ShowCourse", "Course", new {id = cvm.Id});
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