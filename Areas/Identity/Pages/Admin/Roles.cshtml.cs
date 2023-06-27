using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using ChatApp.Areas.Identity.Data;

namespace ChatApp.Areas.Identity.Pages.Admin
{
    //[Authorize(Policy = "RequireAdmin")]
    public class RolesModel : PageModel
    {
        public RolesModel(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            Roles = new List<string>();
        }

        public UserManager<ApplicationUser> UserManager { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public IList<string> Roles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                //Redirect to NotFound
                return RedirectToPage("/");
            }
            ApplicationUser user = await UserManager.FindByIdAsync(Id);
            Roles = await UserManager.GetRolesAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAddRoleAsync([Required] string roleName)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(Id);

            if (ModelState.IsValid)
            {
                var result = await UserManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, err.Description);
                    }
                }
            }
            Roles = await UserManager.GetRolesAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteRoleAsync([Required] string roleName)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(Id);
            if (ModelState.IsValid)
            {
                var result = await UserManager.RemoveFromRoleAsync(user, roleName);
            }

            Roles = await UserManager.GetRolesAsync(user);
            return RedirectToPage();

        }
    }
}