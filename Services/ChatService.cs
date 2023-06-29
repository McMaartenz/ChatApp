using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using ChatApp.Data.Entities;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Channels;
using Channel = ChatApp.Data.Entities.Channel;

namespace ChatApp.Services
{
	public class ChatService
	{
		private readonly UserService _userService;
		private readonly ChatDataService _dataService;

		public ChatService(
			UserService userService,
			ChatDataService dataService)
		{
			_dataService = dataService;
			_userService = userService;
		}

		public async Task<Channel?> GetChannel(int channelId)
		{
			Channel? channel = await _dataService.FindChannelAsync(channelId);
			return channel;
		}

		public async Task<Channel[]> GetChannels()
		{
			Channel[] channels = await _dataService.GetChannelsWithoutMessages();

			if (channels.Length > 0)
			{
				return channels;
			}

			// Assert a channel exists
			Channel channel = new()
			{
				Topic = "General"
			};

			await _dataService.AddChannel(channel);

			channel.Messages = new List<Message>();
			return new Channel[] { channel };
		}

		public async Task<Message[]> GetMessages(int channelId, int amount = 50)
		{
			return await _dataService.GetLastMessages(channelId, Math.Max(1, Math.Min(128, amount)));
		}

		public async Task<int> PostMessage(Message message)
		{
			if (!await _dataService.ChannelExists(message.ChannelId))
			{
				return 0;
			}

			if (!await _dataService.UserExists(message.UserId))
			{
				return 0;
			}

			if (message.Content.Length > 255)
			{
				return 0;
			}

			if (message.Timestamp > DateTime.UtcNow)
			{
				return 0;
			}

			message.Channel = null;
			message.User = null;

			await _dataService.AddMessage(message);
			return message.Id;
		}

		public async Task<bool> DeleteMessage(int userId, int messageId)
		{
			Message? message = await _dataService.GetMessage(messageId);
			if (message is null)
			{
				return false;
			}

			if (message.UserId != userId)
			{
				return false;
			}

			await _dataService.RemoveMessage(message);
			return true;
		}

		public async Task<int> PostChannel(string topic)
		{
			if (topic.Length > 36)
			{
				return 0;
			}

			Channel channel = new()
			{
				Topic = topic
			};

			// return id?
			await _dataService.AddChannel(channel);
			return channel.Id;
		}

		public async Task<User?> GetUser(int userId)
		{
			User? user = await _dataService.GetUser(userId);
			return user;
		}

		public async Task<User> GetUserId(string userId)
		{
			User? user = await _dataService.GetUserFromStringId(userId);
			if (user is null)
			{
				ApplicationUser appUser = await _userService.Get(userId);
				string userName = $"{appUser.FirstName}{appUser.LastName}";

				user = new()
				{
					Timestamp = DateTime.UtcNow,
					UserName = userName,
					UserId = userId,
				};

				// return id?
				await _dataService.AddUser(user);
			}

			return user;
		}
	}
}
