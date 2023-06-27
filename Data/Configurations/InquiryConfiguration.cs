using ChatApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Data.Configurations
{
	public class InquiryConfiguration : IEntityTypeConfiguration<Inquiry>
	{
		public void Configure(EntityTypeBuilder<Inquiry> builder)
		{
			builder.Property(x => x.Topic).HasMaxLength(200);
			builder.Property(x => x.Email).HasMaxLength(256);
			builder.Property(x => x.Message).HasMaxLength(600);
			builder.Property(x => x.Time).IsRequired();
		}
	}
}
