using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using ChatApp.Data.Entities;
using ChatApp.Exceptions;
using ChatApp.Services;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Immutable;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using Channel = ChatApp.Data.Entities.Channel;

namespace ChatApp.Hubs
{
	public class ChatHub : Hub
	{
		private readonly ILogger<ChatHub> _logger;
		private readonly HttpClient _http;

		private readonly string _baseUrl = "https://localhost:7290/api/ChatApi/"; // or: http://localhost:5246

		public ChatHub(
			ILogger<ChatHub> logger,
			HttpClient http)
		{
			_logger = logger;
			_http = http;
			_http.BaseAddress = new Uri(_baseUrl);
			_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("APIKey", "ca798148-9038-402a-95c7-c7905e694270");
		}

		private async Task VerifyUser(int userId)
		{
			if (Context.UserIdentifier is null)
			{
				throw new AuthenticationException();
			}

			User? user = await GetUser(userId) ?? throw new AuthenticationException();
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

			int userId = await Get<int>($"users/{Context.UserIdentifier}/id");
			return userId;
		}

		public async Task<User?> GetUser(int userId)
		{
			User? user = await Get<User>($"users/{userId}");
			return user;
		}

		private async Task<T?> Get<T>(string route)
		{
			HttpResponseMessage response = await _http.GetAsync(route);
			response.EnsureSuccessStatusCode();

			T? entities = await response.Content.ReadFromJsonAsync<T>();
			return entities;
		}

		private async Task<int?> Post<T>(string route, T entity)
		{
			HttpResponseMessage response = await _http.PostAsJsonAsync(route, entity);
			response.EnsureSuccessStatusCode();

			try
			{
				int result = await response.Content.ReadFromJsonAsync<int>();
				return result;
			}
			catch
			{
				return null;
			}
		}

		private async Task Delete(string route)
		{
			HttpResponseMessage response = await _http.DeleteAsync(route);
			response.EnsureSuccessStatusCode();
		}

		public async Task<int[]> GetChannels()
		{
			Channel[] channels = await Get<Channel[]>("channels") ?? Array.Empty<Channel>();
			return channels.Select(c => c.Id).ToArray();
		}

		public async Task<Channel?> GetChannel(int channelId)
		{
			Channel? channel = await Get<Channel>($"channels/{channelId}");
			return channel;
		}

		public async Task<Message[]> GetLastMessages(int channelId)
		{
			Message[] messages = await Get<Message[]>($"channels/{channelId}/messages") ?? Array.Empty<Message>();
			return messages;
		}

		public async Task SendMessage(int userId, int channelId, string messageContent)
		{
			await VerifyUser(userId);

			Message message = new()
			{
				User = await GetUser(userId) ?? throw new AuthenticationException(),
				UserId = userId,
				Timestamp = DateTime.UtcNow,
				Content = messageContent.Truncate(255),
				Deleted = false,
				ChannelId = channelId,
				Channel = await GetChannel(channelId) ?? throw new InvalidChannelException()
			};

			int? messageId = await Post("messages", message);
			message.Id = (int)messageId;
			
			await Clients.All.SendAsync($"ReceiveMessage-{channelId}", message);
		}

		public async Task<bool> DeleteMessage(int userId, int messageId)
		{
			await VerifyUser(userId);

			await Delete($"messages/{userId}/{messageId}");
			await Clients.All.SendAsync("DeleteMessage", messageId);
			return true;
		}

		public async Task<int> CreateChannel(int userId, string topic)
		{
			await VerifyUser(userId);

			int? channelId = await Post("channels", topic);
			return (int)channelId;
		}
	}
}