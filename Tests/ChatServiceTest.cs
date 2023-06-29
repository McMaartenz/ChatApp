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
using System.Runtime.CompilerServices;
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

		[Theory]
		[InlineData(-5, 1)]
		[InlineData(5, 5)]
		[InlineData(50, 50)]
		[InlineData(255, 128)]
		public async Task GetsMessages(int amount, int expected)
		{
			// Arrange
			_mockDataService.Setup(x => x.GetLastMessages(1, It.IsAny<int>())).ReturnsAsync(Array.Empty<Message>());

			// Act
			_ = await _mockChatService.Object.GetMessages(1, amount);

			// Assert
			_mockDataService.Verify(x => x.GetLastMessages(1, expected), Times.Once);
		}

		[Fact]
		public async Task CanPostMessage()
		{
			// Arrange
			Message message = new()
			{
				Id = 0,
				UserId = 255,
				ChannelId = 128,
				Content = "Hallo",
				Timestamp = DateTime.UtcNow,
			};

			_mockDataService.Setup(x => x.AddMessage(message)).Returns(Task.CompletedTask).Callback((Message msg) =>
			{
				msg.Id = 513;
			});

			_mockDataService.Setup(x => x.ChannelExists(128)).ReturnsAsync(true);
			_mockDataService.Setup(x => x.UserExists(255)).ReturnsAsync(true);

			// Act
			int messageId = await _mockChatService.Object.PostMessage(message);

			// Assert
			Assert.Equal(513, messageId);
			_mockDataService.Verify(x => x.AddMessage(message), Times.Once);
		}

		[Fact]
		public async Task DeniesMessageWithNonExistingChannel()
		{
			// Arrange
			Message message = new()
			{
				Id = 0,
				UserId = 255,
				ChannelId = 9999,
				Content = "Hallo",
				Timestamp = DateTime.UtcNow,
			};

			_mockDataService.Setup(x => x.AddMessage(message)).Returns(Task.CompletedTask).Callback((Message msg) =>
			{
				msg.Id = 513;
			});

			_mockDataService.Setup(x => x.ChannelExists(128)).ReturnsAsync(true);
			_mockDataService.Setup(x => x.UserExists(255)).ReturnsAsync(true);

			// Act
			int messageId = await _mockChatService.Object.PostMessage(message);

			// Assert
			Assert.Equal(0, messageId);
			_mockDataService.Verify(x => x.AddMessage(message), Times.Never);
		}

		[Fact]
		public async Task DeniesMessageWithNonExistingUser()
		{
			// Arrange
			Message message = new()
			{
				Id = 0,
				UserId = 9999,
				ChannelId = 128,
				Content = "Hallo",
				Timestamp = DateTime.UtcNow,
			};

			_mockDataService.Setup(x => x.AddMessage(message)).Returns(Task.CompletedTask).Callback((Message msg) =>
			{
				msg.Id = 513;
			});

			_mockDataService.Setup(x => x.ChannelExists(128)).ReturnsAsync(true);
			_mockDataService.Setup(x => x.UserExists(255)).ReturnsAsync(true);

			// Act
			int messageId = await _mockChatService.Object.PostMessage(message);

			// Assert
			Assert.Equal(0, messageId);
			_mockDataService.Verify(x => x.AddMessage(message), Times.Never);
		}

		[Fact]
		public async Task DeniesMessageWithTooLongContent()
		{
			// Arrange
			Message message = new()
			{
				Id = 0,
				UserId = 255,
				ChannelId = 128,
				Content = new string('x', 256),
				Timestamp = DateTime.UtcNow,
			};

			_mockDataService.Setup(x => x.AddMessage(message)).Returns(Task.CompletedTask).Callback((Message msg) =>
			{
				msg.Id = 513;
			});

			_mockDataService.Setup(x => x.ChannelExists(128)).ReturnsAsync(true);
			_mockDataService.Setup(x => x.UserExists(255)).ReturnsAsync(true);

			// Act
			int messageId = await _mockChatService.Object.PostMessage(message);

			// Assert
			Assert.Equal(0, messageId);
			_mockDataService.Verify(x => x.AddMessage(message), Times.Never);
		}

		[Fact]
		public async Task DeniesMessageFromTheFuture()
		{
			// Arrange
			Message message = new()
			{
				Id = 0,
				UserId = 255,
				ChannelId = 128,
				Content = "Hallo",
				Timestamp = DateTime.UtcNow.AddDays(1),
			};

			_mockDataService.Setup(x => x.AddMessage(message)).Returns(Task.CompletedTask).Callback((Message msg) =>
			{
				msg.Id = 513;
			});

			_mockDataService.Setup(x => x.ChannelExists(128)).ReturnsAsync(true);
			_mockDataService.Setup(x => x.UserExists(255)).ReturnsAsync(true);

			// Act
			int messageId = await _mockChatService.Object.PostMessage(message);

			// Assert
			Assert.Equal(0, messageId);
			_mockDataService.Verify(x => x.AddMessage(message), Times.Never);
		}

		[Theory]
		[InlineData(55, 98, true)]
		[InlineData(55, 99, false)]
		[InlineData(54, 98, false)]
		public async Task DeletesMessage(int userId, int messageId, bool expected)
		{
			// Arrange
			_mockDataService.Setup(x => x.RemoveMessage(It.IsAny<Message>())).Returns(Task.CompletedTask);
			_mockDataService.Setup(x => x.GetMessage(98)).ReturnsAsync(new Message() { UserId = 55 });

			// Act
			bool actual = await _mockChatService.Object.DeleteMessage(userId, messageId);

			// Assert
			Assert.Equal(expected, actual);
			_mockDataService.Verify(x => x.RemoveMessage(It.IsAny<Message>()), expected ? Times.Once : Times.Never);
		}

		[Fact]
		public async Task PostsChannel()
		{
			// Arrange
			int expected = 15;
			_mockDataService.Setup(x => x.AddChannel(It.IsAny<Channel>()))
				.Returns(Task.CompletedTask)
				.Callback((Channel channel) =>
				{
					channel.Id = expected;
				});

			// Act
			int channelId = await _mockChatService.Object.PostChannel("Topic");

			// Assert
			Assert.Equal(expected, channelId);
		}
	}
}
