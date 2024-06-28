using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using ChatApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
	public class UserService
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public UserService(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public virtual async Task<ApplicationUser> Get(string userId)
		{
			return await _userManager.FindByIdAsync(userId);
		}

		public virtual async Task<bool> HasRole(string userId, string role)
		{
			ApplicationUser appUser = await Get(userId);
			if (appUser == null)
			{
				return false;
			}

			return await _userManager.IsInRoleAsync(appUser, role);
		}
	}
}
