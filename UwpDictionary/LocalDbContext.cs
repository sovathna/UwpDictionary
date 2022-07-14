using Microsoft.EntityFrameworkCore;

namespace UwpDictionary
{
	public sealed class LocalDbContext : DbContext
	{

		public DbSet<History> Histories​ { get; set; }
		public DbSet<Bookmark> Bookmarks { get; set; }

		public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}
	}
}