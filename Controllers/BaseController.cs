using ChatApp.Areas.Identity.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChatApp.Controllers
{
	public class BaseController : Controller
	{
		protected readonly SignInManager<ApplicationUser> _signInManager;
		protected readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<BaseController> _logger;

        public BaseController(
            UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager, 
            ILogger<BaseController> logger)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        private ViewResult SetCookies(ViewResult result)
		{
			result.ViewData["requestCount"] = Request.Cookies["requestCount"] ?? "0";
			result.ViewData["useCookies"] = Request.Cookies["gdpr"] ?? "";
            
            return result;
        }

        public override ViewResult View()
        {
            ViewResult result = SetCookies(base.View());
            return result;
        }

		public override ViewResult View(string? viewName, object? model)
		{
			ViewResult result = SetCookies(base.View(viewName, model));
			return result;
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}