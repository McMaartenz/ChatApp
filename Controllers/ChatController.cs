using ChatApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public ChatController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<HomeController> logger)
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
