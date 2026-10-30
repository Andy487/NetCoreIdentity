
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetCoreIdentity.Models;

namespace NetCoreIdentity.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DeleteUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserModel(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser UserToDelete { get; set; } = default!;

        [BindProperty]
        public string Id { get; set; } = string.Empty;

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

            UserToDelete = user;
            Id = user.Id;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            // Prevent an Admin from deleting their own account
            var currentUserId = _userManager.GetUserId(User);

            if (user.Id == currentUserId)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "You cannot delete your own account.");

                UserToDelete = user;

                return Page();
            }

            // Check whether the target user is an active Admin
            var userRoles = await _userManager.GetRolesAsync(user);

            bool isActiveAdmin =
                user.IsActive && userRoles.Contains("Admin");

            if (isActiveAdmin)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");

                int activeAdminCount = admins.Count(u => u.IsActive);

                // Prevent deletion of the last active Admin
                if (activeAdminCount <= 1)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "You cannot delete the last active System Administrator.");

                    UserToDelete = user;

                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                UserToDelete = user;

                return Page();
            }

            TempData["Success"] = "User deleted successfully.";

            return RedirectToPage("/Admin/Users");
        }
    }
}
