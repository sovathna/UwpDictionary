using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel;

namespace UwpDictionary
{
	public sealed class WordsDbContext : DbContext
	{

		public DbSet<Word> Words { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var dbPath = Package.Current.InstalledPath + "/Assets/Databases/khdict.sqlite";
			var connectionString = "Data Source=" + dbPath;
			optionsBuilder
				.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors()
				.UseSqlite(connectionString);
		}
	}
}