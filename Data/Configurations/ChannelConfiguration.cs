using ChatApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Data.Configurations
{
	public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
	{
		public void Configure(EntityTypeBuilder<Channel> builder)
		{
			builder.Property(x => x.Topic).HasMaxLength(16);
		}
	}
}
