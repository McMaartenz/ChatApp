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
		private readonly Mock<ChatDataService> _mockDataService;
		private readonly Mock<UserService> _mockUserService;

		private readonly Mock<ChatService> _mockChatService;

		// Setup
		public ChatServiceTest()
		{
			_mockDataService = new(null);
			_mockUserService = new(null);

			_mockChatService = new(_mockUserService.Object, _mockDataService.Object);
		}

		[Fact]
		public async Task GetsChannel()
		{
			// Arrange
			Channel expected = new()
			{
				Id = 5,
				Topic = "value"
			};

			_mockDataService.Setup(x => x.FindChannelAsync(5)).ReturnsAsync(expected);

			// Act
			Channel? actual = await _mockChatService.Object.GetChannel(expected.Id);

			// Assert
			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task GetsNullOnInvalidChannel()
		{
			// Arrange
			_mockDataService.Setup(x => x.FindChannelAsync(2)).ReturnsAsync(null as Channel);

			// Act
			Channel? actual = await _mockChatService.Object.GetChannel(2);

			// Assert
			Assert.Null(actual);
		}

		[Fact]
		public async Task ReturnsChannels()
		{
			// Arrange
			Channel[] expected =
			{
				new() { Id = 1, Topic = "abc" },
				new() { Id = 2, Topic = "def" },
				new() { Id = 3, Topic = "ghi" }
			};

			_mockDataService.Setup(x => x.GetChannelsWithoutMessages()).ReturnsAsync(expected);

			// Act
			Channel[] actual = await _mockChatService.Object.GetChannels();

			// Assert
			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task CreatesGeneralChannelIfNoneExists()
		{
			// Arrange
			_mockDataService.Setup(x => x.GetChannelsWithoutMessages()).ReturnsAsync(Array.Empty<Channel>());

			// Act
			Channel[] actual = await _mockChatService.Object.GetChannels();

			// Assert
			Assert.NotEmpty(actual);
			_mockDataService.Verify(x => x.AddChannel(actual[0]), Times.Once);
		}
	}
}
