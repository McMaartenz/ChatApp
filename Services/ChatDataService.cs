﻿using ChatApp.Data;
using ChatApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using Channel = ChatApp.Data.Entities.Channel;

namespace ChatApp.Services
{
	public class ChatDataService
	{
		private readonly ChatAppDbCtx _dbCtx;

		public ChatDataService(ChatAppDbCtx dbCtx)
		{
			_dbCtx = dbCtx;
		}

		public virtual async Task<Channel?> FindChannelAsync(int channelId)
		{
			return await _dbCtx.Channels.FindAsync(channelId);
		}

		public virtual async Task<Channel[]> GetChannelsWithoutMessages()
		{
			return await _dbCtx.Channels
				   .Select(c => new Channel
				   {
					   Id = c.Id,
					   Topic = c.Topic
				   })
				   .ToArrayAsync();
		}

		public virtual async Task AddChannel(Channel channel)
		{
			await _dbCtx.AddAsync(channel);
			await _dbCtx.SaveChangesAsync();
		}

		public virtual async Task<Message[]> GetLastMessages(int channelId, int amount)
		{
			return await _dbCtx.Messages
				.Where(m => m.ChannelId == channelId)
				.OrderBy(m => m.Timestamp)
				.Take(amount)
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

		public virtual async Task<bool> ChannelExists(int channelId)
		{
			return await _dbCtx.Channels.AnyAsync(c => c.Id == channelId);
		}

		public virtual async Task<bool> UserExists(int userId)
		{
			return await _dbCtx.Users.AnyAsync(u => u.Id == userId);
		}

		public virtual async Task AddMessage(Message message)
		{
			await _dbCtx.AddAsync(message);
			await _dbCtx.SaveChangesAsync();
		}

		public virtual async Task<Message?> GetMessage(int messageId)
		{
			return await _dbCtx.Messages.FindAsync(messageId);
		}

		public virtual async Task RemoveMessage(Message message)
		{
			_dbCtx.Messages.Remove(message);
			await _dbCtx.SaveChangesAsync();
		}

		public virtual async Task<User?> GetUser(int userId)
		{
			return await _dbCtx.Users.FindAsync(userId);
		}

		public virtual async Task<User?> GetUserFromStringId(string userId)
		{
			return await _dbCtx.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
		}

		public virtual async Task AddUser(User user)
		{
			await _dbCtx.Users.AddAsync(user);
			await _dbCtx.SaveChangesAsync();
		}
	}
}
