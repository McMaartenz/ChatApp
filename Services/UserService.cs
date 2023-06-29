using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using ChatApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
	public class UserService
	{
		private readonly ChatAppDbCtx _chatAppDbCtx;
		private readonly ApplicationDbContext _appDbCtx;
		protected readonly SignInManager<ApplicationUser> _signInManager;
		protected readonly UserManager<ApplicationUser> _userManager;

		public UserService(
			ChatAppDbCtx chatAppDbCtx,
			ApplicationDbContext appDbCtx,
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager)
		{
			_chatAppDbCtx = chatAppDbCtx;
			_appDbCtx = appDbCtx;
			_signInManager = signInManager;
			_userManager = userManager;
		}

		public async Task<ApplicationUser> Get(string userId)
		{
			return await _userManager.FindByIdAsync(userId);
		}
	}
}
