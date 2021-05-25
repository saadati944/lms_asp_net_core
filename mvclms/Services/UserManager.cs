using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using mvclms.Data;
using mvclms.Models;
using mvclms.ViewModels;

namespace mvclms.Services
{
    public class UserManager
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
            Task<IdentityResult> createRes =  _userManager.CreateAsync(p, person.Password);
            createRes.Wait();
            return createRes.Result.Succeeded && AddToRole(p, person.PersonMode);
        }
        
        public bool AddToRole(Person person, string role)
        {
            Task<bool> res = _roleManager.RoleExistsAsync(role);
            res.Wait();
            if (!res.Result)
                _roleManager.CreateAsync(new IdentityRole{Name = role}).Wait();
            var x = _userManager.AddToRoleAsync(person, role);
            x.Wait();
            return x.Result.Succeeded;
        }

        public bool Login(LoginViewModel login)
        {
            var result = _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, false);
            result.Wait();
            return result.Result.Succeeded;
        }

        public void Logout()
        {
            _signInManager.SignOutAsync().Wait();
        }
    }
}