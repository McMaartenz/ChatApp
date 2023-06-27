using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChatApp.Areas.Identity.Pages.Admin
{
    [Authorize(Policy = "RequireAdmin")]
    public class UsersModel : PageModel
    {
        public ApplicationDbContext _dbCtx { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }
                        = Enumerable.Empty<ApplicationUser>();

        public UsersModel(ApplicationDbContext dbCtx)
        {
            _dbCtx = dbCtx;
        }

        public void OnGet()
        {
            Users = _dbCtx.Users.ToList();
        }
    }
}
