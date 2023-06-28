using ChatApp.Data.Configurations;
using ChatApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
	public class ChatAppDbCtx : DbContext
	{
		public ChatAppDbCtx(DbContextOptions<ChatAppDbCtx> options) : base(options) { }

		public DbSet<User> Users => Set<User>();
		public DbSet<Channel> Channels => Set<Channel>();
		public DbSet<Message> Messages => Set<Message>();

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfiguration(new ChannelConfiguration());
			builder.ApplyConfiguration(new MessageConfiguration());
			builder.ApplyConfiguration(new ChannelConfiguration());
			builder.ApplyConfiguration(new UserConfiguration());
		}
	}
}
