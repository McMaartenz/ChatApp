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
    public class ClaimsModel : PageModel
    {
        public ClaimsModel(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public IEnumerable<Claim> Claims { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                //Redirect to NotFound
                return RedirectToPage("/");
            }
            ApplicationUser user = await UserManager.FindByIdAsync(Id);
            Claims = await UserManager.GetClaimsAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAddClaimAsync([Required] string type,
                                                             [Required] string value)
        {

            ApplicationUser user = await UserManager.FindByIdAsync(Id);

            if (ModelState.IsValid)
            {
                Claim claim = new(type, value);
                var result = await UserManager.AddClaimAsync(user, claim);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, err.Description);
                    }
                }
            }
            Claims = await UserManager.GetClaimsAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditClaimAsync([Required] string type,
                                                              [Required] string value,
                                                              [Required] string oldValue)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(Id);
            if (ModelState.IsValid)
            {
                Claim claimNew = new(type, value);
                Claim claimOld = new(type, oldValue);
                var result = await UserManager.ReplaceClaimAsync(user, claimOld, claimNew);
            }
            Claims = await UserManager.GetClaimsAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteClaimAsync([Required] string type,
                                                                [Required] string value)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(Id);
            if (ModelState.IsValid)
            {
                Claim claim = new(type, value);
                var result = await UserManager.RemoveClaimAsync(user, claim);
            }
            Claims = await UserManager.GetClaimsAsync(user);
            return RedirectToPage();

        }
    }
}