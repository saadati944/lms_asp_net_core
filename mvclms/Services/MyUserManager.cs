using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mvclms.Data;
using mvclms.Models;
using mvclms.ViewModels;

namespace mvclms.Services
{
    public class MyUserManager : IUserManager
    {
        //todo : use role manager instead of isteacher in person model
        private readonly ILogger<MyUserManager> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Person> _userManager;
        private readonly SignInManager<Person> _signInManager;

        public bool isTeacher = false;
        public bool isStudent = false;

        private Person _user;

        public MyUserManager(ApplicationDbContext dbContext, UserManager<Person> userManager,
            SignInManager<Person> signInManager, ILogger<MyUserManager> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public void AddNavigationBarButtons(dynamic ViewBag, ClaimsPrincipal cp)
        {
            if (ViewBag.navbar is null)
                ViewBag.navbar = new List<NavbarButton>();
            var user = GetUser(cp);
            if (user is null)
            {
                ViewBag.navbar.Add(new NavbarButton
                {
                    Title = "Login",
                    Controller = "Users",
                    Action = "Login"
                });

                ViewBag.navbar.Add(new NavbarButton
                {
                    Title = "Signup",
                    Controller = "Users",
                    Action = "SignUp"
                });
                return;
            }

            ViewBag.navbar.Add(new NavbarButton
            {
                Title = "Profile",
                Controller = "Users",
                Action = "Profile",
                Id = GetUser(cp).Id
            });

            ViewBag.navbar.Add(new NavbarButton
            {
                Title = "Logout",
                Controller = "Users",
                Action = "Logout"
            });
        }

        public IdentityResult CreateUser(PersonViewModel person)
        {
            Person p = new Person
            {
                UserName = person.UserName,
                FirstName = person.FirstName,
                LastName = person.LastName,
                IsTeacher = person.PersonMode == PersonViewModel.PersonModes[0].Text //teacher // todo: fix this ...
            };

            var result = _userManager.CreateAsync(p, person.Password).GetAwaiter().GetResult();
            if (!result.Succeeded)
                return result;

            // todo: again check roles i am pretty sure it will work next time ...
            // todo: this line throws error !!!
            // check it and fix the error later
            // AddToRole(p, person.PersonMode);

            return result;
        }

        // public bool AddToRole(Person person, string role)
        // {
        //     if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
        //         _roleManager.CreateAsync(new IdentityRole{Name = role}).Wait();
        //     return _userManager.AddToRoleAsync(person, role).GetAwaiter().GetResult().Succeeded;
        // }

        public bool Login(LoginViewModel login)
        {
            Logout();

            return _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, false)
                .GetAwaiter().GetResult().Succeeded;
        }

        public void Logout()
        {
            _signInManager.SignOutAsync().Wait();
        }

        public Person GetUser(ClaimsPrincipal claimsPrincipal)
        {
            if (_user is null)
            {
                _user = _userManager.GetUserAsync(claimsPrincipal).GetAwaiter().GetResult();

                if (_user is not null)
                {
                    if (_user.IsTeacher)
                        isTeacher = true;
                    else
                        isStudent = true;
                }
            }

            return _user;
        }

        public Person GetUser(string id)
        {
            return _dbContext.Users.Where(x => x.Id == id).Include(x => x.Courses).Include(x => x.StudentCourses)
                .FirstOrDefault();
        }

        public Person GetUserByUsername(string username)
        {
            return _dbContext.Users.Where(x => x.UserName == username).Include(x => x.Courses)
                .Include(x => x.StudentCourses)
                .FirstOrDefault();
        }

        public void CheckoutCourse(ClaimsPrincipal claimsPrincipal, Course c, Person user = null)
        {
            user ??= GetUser(claimsPrincipal);

            user.Courses.Add(c);
            StudentCourse sc = new StudentCourse {Course = c, Student = user};
            c.StudentCourses.Add(sc);
            _dbContext.StudentCourses.Add(sc);
            _dbContext.SaveChanges();
        }
    }
}