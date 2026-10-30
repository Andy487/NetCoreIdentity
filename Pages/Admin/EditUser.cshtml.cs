using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetCoreIdentity.Models;

namespace NetCoreIdentity.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditUserModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public EditUserViewModel UserVM { get; set; } = new();

        public List<SelectListItem> Roles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            UserVM = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = userRoles.FirstOrDefault() ?? string.Empty,
                IsActive = user.IsActive
            };

            await LoadRoles();

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            await LoadRoles();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UserVM.Id);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            bool isCurrentlyActiveAdmin =
                user.IsActive && currentRoles.Contains("Admin");

            bool removingAdminProtection =
                !UserVM.IsActive || UserVM.Role != "Admin";

            // Protect the last active Admin account
            if (isCurrentlyActiveAdmin && removingAdminProtection)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");

                int activeAdminCount = admins.Count(u => u.IsActive);

                if (activeAdminCount <= 1)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "You cannot deactivate or remove the Admin role from the last active System Administrator.");

                    return Page();
                }
            }

            user.FullName = UserVM.FullName;
            user.Email = UserVM.Email;
            user.UserName = UserVM.Email;
            user.IsActive = UserVM.IsActive;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            if (currentRoles.Any())
            {
                var removeRoleResult =
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeRoleResult.Succeeded)
                {
                    foreach (var error in removeRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
            }

            var addRoleResult =
                await _userManager.AddToRoleAsync(user, UserVM.Role);

            if (!addRoleResult.Succeeded)
            {
                foreach (var error in addRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            TempData["Success"] = "User updated successfully.";

            return RedirectToPage("/Admin/Users");
        }


        private async Task LoadRoles()
        {
            Roles = await Task.FromResult(
                _roleManager.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name!,
                        Text = r.Name!
                    })
                    .ToList());
        }
    }
}