
using Microsoft.AspNetCore.Identity;
using NetCoreIdentity.Models;

namespace NetCoreIdentity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<LoginResult> LoginAsync(
            string email,
            string password,
            bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return LoginResult.InvalidCredentials;

            if (!user.IsActive)
                return LoginResult.Deactivated;

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                password,
                rememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
                return LoginResult.Success;

            if (result.IsLockedOut)
                return LoginResult.LockedOut;

            return LoginResult.InvalidCredentials;
        }
    }
}
