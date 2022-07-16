using Microsoft.EntityFrameworkCore;

namespace UwpDictionary
{
    public sealed class WordsDbContext : DbContext
    {
        public WordsDbContext(DbContextOptions<WordsDbContext> options) : base(options)
        {
        }

        public DbSet<Word> Words { get; set; }
    }
}