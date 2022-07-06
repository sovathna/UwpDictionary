using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Windows.Storage;

namespace UwpDictionary
{
	public sealed class LocalDbContext : DbContext
	{

		public LocalDbContext()
		{
			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var dbPath = ApplicationData.Current.LocalFolder.Path + "/local.sqlite";
			var connectionString = "Data Source=" + dbPath;
			optionsBuilder
				.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors()
				.UseSqlite(connectionString);
		}
	}
}