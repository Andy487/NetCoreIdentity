using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetCoreIdentity.Models;

namespace NetCoreIdentity.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public List<UserViewModel> Users { get; set; } = new();

        public UsersModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            Users = new List<UserViewModel>();

            foreach (var user in _userManager.Users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                Users.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = roles.FirstOrDefault() ?? "No Role",
                    CreatedDate = user.CreatedDate
                });
            }
        }
    }
}