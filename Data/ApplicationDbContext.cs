using ChatApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ChatApp.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

		protected override void OnModelCreating(ModelBuilder builder)
        {
			base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new()
                {
                    Id = "b3c7c71d-44ac-4f8c-8b65-9c9513d9affc",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                },
                new()
                {
                    Id = "6557c6c9-85c9-48ab-9f80-22d9011c4985",
                    Name = "ChatModerator",
                    NormalizedName = "CHATMODERATOR"
                });

            builder.Entity<ApplicationUser>()
				.Property(e => e.FirstName)
				.HasMaxLength(255);

			builder.Entity<ApplicationUser>()
				.Property(e => e.LastName)
				.HasMaxLength(255);
		}
	}
}
