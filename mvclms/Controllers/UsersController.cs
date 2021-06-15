using System.Linq;
using Microsoft.AspNetCore.Mvc;
using mvclms.Data;
using mvclms.Models;
using mvclms.Services;
using mvclms.ViewModels;

namespace mvclms.Controllers
{
    public class UsersController : Controller
    {
        private readonly MyUserManager _userManager;
        private readonly ApplicationDbContext _dbContext;

        public UsersController(MyUserManager userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        private void addUserNavbar()
        {
            _userManager.AddNavigationBarButtons(ViewBag, User);
        }

        public IActionResult Logout()
        {
            _userManager.Logout();
            return View();
        }

        [HttpGet]
        public IActionResult SignUp(int? temp)
        {
            if(temp is not null)
                return Ok(_dbContext.Users.ToList());
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(PersonViewModel person)
        {
            if (!ModelState.IsValid)
                return View();

            var result = _userManager.CreateUser(person);
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View();
            }
            
            return RedirectToAction("Login", "Users");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel logindata)
        {
            if (!ModelState.IsValid)
                return View();
            
            _userManager.Login(logindata);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile(string? id)
        {
            addUserNavbar();
            if(_userManager.isTeacher)
            {
                ViewBag.navbar.Add(new NavbarButton
                {
                    Title = "CreatedCourses",
                    Controller = "Course",
                    Action = "TeacherCourses"
                });
                ViewBag.navbar.Add(new NavbarButton
                {
                    Title = "NewCourse",
                    Controller = "Course",
                    Action = "CreateCourse"
                });
            }
            else if(_userManager.isStudent)
            {
                ViewBag.navbar.Add(new NavbarButton
                {
                    Title = "Courses",
                    Controller = "Course",
                    Action = "StudentCourses"
                });
            }
            Person p;
            if (id is null)
                p = _userManager.GetUser(_userManager.GetUser(User).Id);
            else
                p = _userManager.GetUser(id);
            
            return View(p);
        }
    }
}