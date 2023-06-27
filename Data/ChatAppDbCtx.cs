using ChatApp.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
	public class ChatAppDbCtx : DbContext
	{
		public ChatAppDbCtx(DbContextOptions<ChatAppDbCtx> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// builder.ApplyConfiguration(new ___Configuration());
		}
	}
}
