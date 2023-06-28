using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ChatApp.Data.Entities;

namespace ChatApp.Data.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.Property(x => x.UserId).IsRequired();
			builder.Property(x => x.Timestamp).IsRequired();
		}
	}
}
