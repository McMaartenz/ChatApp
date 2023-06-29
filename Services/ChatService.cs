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
		private readonly ChatAppDbCtx _chatAppDbCtx;

		public ChatService(
			UserService userService,
			ChatAppDbCtx chatAppDbCtx)
		{
			_userService = userService;
			_chatAppDbCtx = chatAppDbCtx;
		}

		public async Task<Channel?> GetChannel(int channelId)
		{
			Channel? channel = await _chatAppDbCtx.Channels.FindAsync(channelId);
			return channel;
		}

		public async Task<Channel[]> GetChannels()
		{
			Channel[] channels = await _chatAppDbCtx.Channels
				.Select(c => new Channel
				{
					Id = c.Id,
					Topic = c.Topic
				})
				.ToArrayAsync();

			if (channels.Length > 0)
			{
				return channels;
			}

			// Assert a channel exists
			Channel channel = new()
			{
				Topic = "General"
			};

			await _chatAppDbCtx.AddAsync(channel);
			await _chatAppDbCtx.SaveChangesAsync();

			channel.Messages = new List<Message>();
			return new Channel[] { channel };
		}

		public async Task<Message[]> GetMessages(int channelId, int amount = 50)
		{
			return await _chatAppDbCtx.Messages
				.Where(m => m.ChannelId == channelId)
				.OrderBy(m => m.Timestamp)
				.Take(Math.Min(1, Math.Max(128, amount)))
				.Select(m => new Message
				{
					Id = m.Id,
					Content = m.Content,
					Timestamp = m.Timestamp,
					Deleted = m.Deleted,
					User = m.User,
					UserId = m.UserId,
					ChannelId = m.ChannelId
				}).ToArrayAsync();
		}

		public async Task<int> PostMessage(Message message)
		{
			if (!await _chatAppDbCtx.Channels.AnyAsync(c => c.Id == message.ChannelId))
			{
				return 0;
			}

			if (!await _chatAppDbCtx.Users.AnyAsync(u => u.Id == message.UserId))
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

			await _chatAppDbCtx.AddAsync(message);
			await _chatAppDbCtx.SaveChangesAsync();
			return message.Id;
		}

		public async Task<bool> DeleteMessage(int userId, int messageId)
		{
			Message? message = await _chatAppDbCtx.Messages.FindAsync(messageId);
			if (message is null)
			{
				return false;
			}

			if (message.UserId != userId)
			{
				return false;
			}

			_chatAppDbCtx.Messages.Remove(message);
			await _chatAppDbCtx.SaveChangesAsync();
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

			await _chatAppDbCtx.AddAsync(channel);
			await _chatAppDbCtx.SaveChangesAsync();
			return channel.Id;
		}

		public async Task<User?> GetUser(int userId)
		{
			User? user = await _chatAppDbCtx.Users.FindAsync(userId);
			return user;
		}

		public async Task<User> GetUserId(string userId)
		{
			User? user = await _chatAppDbCtx.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
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

				await _chatAppDbCtx.Users.AddAsync(user);
				await _chatAppDbCtx.SaveChangesAsync();
			}

			return user;
		}
	}
}
