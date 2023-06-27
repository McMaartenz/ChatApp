using ChatApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChatController> logger)
            : base(userManager, signInManager, logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
