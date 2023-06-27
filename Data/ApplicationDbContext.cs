using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
	}
}
