using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using mvclms.Models;
using mvclms.Services;
using mvclms.ViewModels;

namespace mvclms.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager _userManager;

        public UsersController(UserManager userManager)
        {
            _userManager = userManager;
        }


        public IActionResult Logout()
        {
            _userManager.Logout();
            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
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

        public IActionResult Login()
        {
            return null;
        }

        [Authorize(Roles = "Teacher,Student")]
        public IActionResult Profile()
        {
            return View(_userManager.GetUser());
        }

        public IActionResult Profile(string id)
        {
            return View(_userManager.GetUser(id));
        }
    }
}