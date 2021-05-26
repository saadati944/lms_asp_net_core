using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private Person _user;
        
        public UserManager(ApplicationDbContext dbContext, UserManager<Person> userManager,
            SignInManager<Person> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        public IdentityResult CreateUser(PersonViewModel person)
        {
            Person p = new Person
            {
                UserName = person.UserName,
                FirstName = person.FirstName,
                LastName = person.LastName
            };
            
            var result = _userManager.CreateAsync(p, person.Password).GetAwaiter().GetResult();

            AddToRole(p, person.PersonMode);

            return result;
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
            if(_user is null)
                _user = _userManager.GetUserAsync(ClaimsPrincipal.Current).GetAwaiter().GetResult();

            return _user;
        }

        public Person GetUser(string id)
        {
            // todo : add includes according to method inputs
            return _dbContext.Users.FirstOrDefault(x => x.Id == id);
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