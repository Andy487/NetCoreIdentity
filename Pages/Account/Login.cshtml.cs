
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetCoreIdentity.Models;
using NetCoreIdentity.Services;

namespace NetCoreIdentity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAccountService _accountService;

        public LoginModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _accountService.LoginAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe);

            switch (result)
            {
                case LoginResult.Success:
                    return RedirectToPage("/Dashboard");

                case LoginResult.Deactivated:
                    ModelState.AddModelError(
                        string.Empty,
                        "Your account has been deactivated. Please contact the administrator.");
                    break;

                case LoginResult.LockedOut:
                    ModelState.AddModelError(
                        string.Empty,
                        "Your account is temporarily locked due to multiple failed login attempts. Please try again after 5 minutes.");
                    break;

                default:
                    ModelState.AddModelError(
                        string.Empty,
                        "Invalid Email or Password");
                    break;
            }

            return Page();
        }
    }
}
