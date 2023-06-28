using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using ChatApp.Data.Entities;
using ChatApp.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace ChatApp.Hubs
{
	public class ChatHub : Hub
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ChatAppDbCtx _chatAppDbCtx;
		private readonly ILogger<ChatHub> _logger;

		private readonly IMemoryCache _cache;

		public ChatHub(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ChatAppDbCtx chatAppDbCtx,
			ILogger<ChatHub> logger,
			IMemoryCache memoryCache)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_chatAppDbCtx = chatAppDbCtx;
			_logger = logger;
			_cache = memoryCache;
		}

		/* Throws AuthenticationException on failure */
		private async Task VerifyUser(int userId)
		{
			if (Context.UserIdentifier is null)
			{
				throw new AuthenticationException();
			}

			User? user = await _chatAppDbCtx.Users.FindAsync(userId) ?? throw new AuthenticationException();
			if (user.UserId != Context.UserIdentifier)
			{
				throw new AuthenticationException();
			}
		}

		public async Task<int> GetUserId()
		{
			if (Context.UserIdentifier is null)
			{
				return -1;
			}

			User? user = await _chatAppDbCtx.Users.Where(u => u.UserId == Context.UserIdentifier).FirstOrDefaultAsync();
			if (user is null)
			{
				ApplicationUser appUser = await _userManager.FindByIdAsync(Context.UserIdentifier);
				string userName = $"{appUser.FirstName}{appUser.LastName}";

				user = new()
				{
					Timestamp = DateTime.UtcNow,
					UserName = userName,
					UserId = Context.UserIdentifier,
				};

				await _chatAppDbCtx.Users.AddAsync(user);
				await _chatAppDbCtx.SaveChangesAsync();
			}

			return user.Id;
		}

		public async Task<User?> GetUser(int userId)
		{
			string cacheKey = $"userId_{userId}";

			if (_cache.TryGetValue(cacheKey, out User cachedUser))
			{
				return cachedUser;
			}

			User? user = await _chatAppDbCtx.Users.FindAsync(userId);
			if (user is not null)
			{
				_cache.Set(cacheKey, user, TimeSpan.FromMinutes(10));
			}

			return user;
		}

		public async Task<int[]> GetChannels()
		{
			int[] channels = await _chatAppDbCtx.Channels.Select(c => c.Id).ToArrayAsync();
			if (channels.Length > 0)
			{
				return channels;
			}

			Channel channel = new()
			{
				Topic = "General"
			};

			await _chatAppDbCtx.AddAsync(channel);
			await _chatAppDbCtx.SaveChangesAsync();

			return new int[] { channel.Id };
		}

		public async Task<Channel?> GetChannel(int channelId)
		{
			string cacheKey = $"channelId_{channelId}";

			if (_cache.TryGetValue(cacheKey, out Channel cachedChannel))
			{
				return cachedChannel;
			}

			Channel? channel = await _chatAppDbCtx.Channels.FindAsync(channelId);
			if (channel is not null)
			{
				_cache.Set(cacheKey, channel, TimeSpan.FromMinutes(10));
			}

			return channel;
		}

		public async Task<Message[]> GetLastMessages(int channelId)
		{
			Channel? channel = await _chatAppDbCtx.Channels.FindAsync(channelId);
			if (channel is null)
			{
				return Array.Empty<Message>();
			}

			Message[] messages = await _chatAppDbCtx.Messages
				.Where(m => m.Channel == channel)
				.OrderBy(m => m.Timestamp)
				.Take(50)
				.Select(m => new Message /* Exclude Channel */
				{
					Id = m.Id,
					Content = m.Content,
					Timestamp = m.Timestamp,
					Deleted = m.Deleted,
					User = m.User
				})
				.ToArrayAsync();

			return messages;
		}

		public async Task SendMessage(int userId, int channelId, string messageContent)
		{
			await VerifyUser(userId);

			Message message = new()
			{
				UserId = userId,
				Timestamp = DateTime.UtcNow,
				Content = messageContent,
				Deleted = false,
				ChannelId = channelId
			};

			await _chatAppDbCtx.AddAsync(message);
			await _chatAppDbCtx.SaveChangesAsync();

			await Clients.All.SendAsync($"ReceiveMessage-{channelId}", message);
		}

		public async Task<int> CreateChannel(int userId, string topic)
		{
			await VerifyUser(userId);

			Channel channel = new()
			{
				Topic = topic,
			};

			await _chatAppDbCtx.AddAsync(channel);
			await _chatAppDbCtx.SaveChangesAsync();

			return channel.Id;
		}
	}
}