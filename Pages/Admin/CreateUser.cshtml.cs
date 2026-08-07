using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetCoreIdentity.Models;

namespace NetCoreIdentity.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateUserModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public CreateUserViewModel UserVM { get; set; } = new();

        public List<SelectListItem> Roles { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadRoles();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadRoles();

            if (!ModelState.IsValid)
                return Page();

            var user = new ApplicationUser
            {
                FullName = UserVM.FullName,
                UserName = UserVM.Email,
                Email = UserVM.Email,
                CreatedDate = DateTime.Now,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, UserVM.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserVM.Role);

                TempData["Success"] = "User created successfully.";

                return RedirectToPage("/Admin/Users");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return Page();
        }

        private async Task LoadRoles()
        {
            Roles = await Task.FromResult(
                _roleManager.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList());
        }
    }
}