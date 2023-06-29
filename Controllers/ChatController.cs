using ChatApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        public ChatController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChatController> logger)
            : base(userManager, signInManager, logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
