using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ChatApp.Data.Entities;

namespace ChatApp.Data.Configurations
{
	public class MessageConfiguration : IEntityTypeConfiguration<Message>
	{
		public void Configure(EntityTypeBuilder<Message> builder)
		{
			builder.Property(x => x.Content).HasMaxLength(255).IsRequired();
			builder.Property(x => x.Timestamp).IsRequired();
		}
	}
}
