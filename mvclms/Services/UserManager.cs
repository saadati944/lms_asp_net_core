using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using mvclms.Data;
using mvclms.Models;
using mvclms.ViewModels;

namespace mvclms.Services
{
    public class UserManager : IUserManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Person> _userManager;
        private readonly SignInManager<Person> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public UserManager(ApplicationDbContext dbContext, UserManager<Person> userManager,
            SignInManager<Person> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        public bool CreateUser(PersonViewModel person)
        {
            Person p = new Person
            {
                UserName = person.UserName,
                FirstName = person.FirstName,
                LastName = person.LastName
            };
            return _userManager.CreateAsync(p, person.Password).GetAwaiter().GetResult().Succeeded && AddToRole(p, person.PersonMode);
        }
        
        public bool AddToRole(Person person, string role)
        {
            if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                _roleManager.CreateAsync(new IdentityRole{Name = role}).Wait();
            return _userManager.AddToRoleAsync(person, role).GetAwaiter().GetResult().Succeeded;
        }

        public bool Login(LoginViewModel login)
        {
            return _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, false)
                .GetAwaiter().GetResult().Succeeded;
        }

        public void Logout()
        {
            _signInManager.SignOutAsync().Wait();
        }

        public Person GetUser()
        {
            return _userManager.GetUserAsync(ClaimsPrincipal.Current).GetAwaiter().GetResult();
        }

        public void CheckoutCourse(Course c, Person user = null)
        {
            user ??= GetUser();
            
            user.Courses.Add(c);
            StudentCourse sc = new StudentCourse {Course = c, Student = user};
            c.StudentCourses.Add(sc);
            _dbContext.StudentCourses.Add(sc);
            _dbContext.SaveChanges();
        }
    }
}