using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using ChatApp.Data.Entities;
using ChatApp.Middlewares;
using ChatApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Tests
{
	public class ChatServiceTest
	{
		private readonly Mock<UserService> _mockUserService;
		private readonly Mock<ChatAppDbCtx> _mockChatAppDbCtx;
		private readonly Mock<ChatService> _mockChatService;

		// Setup
		public ChatServiceTest()
		{
			_mockUserService = new(null);
			_mockChatAppDbCtx = new(MockBehavior.Default, new DbContextOptions<ChatAppDbCtx>());

			_mockChatService = new(
				MockBehavior.Default,
				_mockUserService.Object,
				_mockChatAppDbCtx.Object);
		}
	}
}
