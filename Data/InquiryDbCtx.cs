using ChatApp.Data.Configurations;
using ChatApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
	public class InquiryDbCtx : DbContext
	{
		public InquiryDbCtx(DbContextOptions<InquiryDbCtx> options) : base(options) {}

		public DbSet<Inquiry> Inquiries => Set<Inquiry>();

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfiguration(new InquiryConfiguration());
		}
	}
}
