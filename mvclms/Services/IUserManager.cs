using Microsoft.AspNetCore.Identity;
using mvclms.Models;
using mvclms.ViewModels;

namespace mvclms.Services
{
    public interface IUserManager
    {
        IdentityResult CreateUser(PersonViewModel person);
        //bool AddToRole(Person person, string role);
        bool Login(LoginViewModel login);
        void Logout();
        Person GetUser();
        Person GetUser(string id);
    }
}