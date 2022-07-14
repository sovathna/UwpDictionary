using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel;

namespace UwpDictionary
{
	public sealed class WordsDbContext : DbContext
	{
		public WordsDbContext(DbContextOptions<WordsDbContext> options):base(options)
        {

        }

		public DbSet<Word> Words { get; set; }
	}
}