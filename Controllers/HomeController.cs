using ChatApp.Areas.Identity.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChatApp.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(
            UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager, 
            ILogger<HomeController> logger)
            : base(userManager, signInManager, logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}