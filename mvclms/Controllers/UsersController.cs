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
        private readonly MyUserManager _myUserManager;
        private readonly ApplicationDbContext _dbContext;

        public UsersController(MyUserManager myUserManager, ApplicationDbContext dbContext)
        {
            _myUserManager = myUserManager;
            _dbContext = dbContext;
        }


        public IActionResult Logout()
        {
            _myUserManager.Logout();
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

            var result = _myUserManager.CreateUser(person);
            
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
            
            _myUserManager.Login(logindata);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile(string? id)
        {
            Person p;
            if (id is null)
                p = _myUserManager.GetUser(_myUserManager.GetUser(User).Id);
            else
                p = _myUserManager.GetUser(id);
            
            return View(p);
        }
    }
}