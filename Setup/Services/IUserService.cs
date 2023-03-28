using Microsoft.AspNetCore.Identity;
using Setup.Models;

namespace Setup.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel registerViewModel);
    }
    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManager;
        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel registerViewModel)
        {
            if (registerViewModel == null)
            {
                throw new NullReferenceException("Register Model is null!");
            }
            var identityUser = new IdentityUser
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email
            };
            return null;
        }
        //identity db tutorial 5/9 vanaf 9:15 verder volgen
    }
}