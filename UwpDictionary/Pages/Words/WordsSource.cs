using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UwpDictionary.Pages.Words
{
    public class WordsSource : IIncrementalSource<WordUi>
    {
        private readonly WordsDbContext _context;
        private readonly LocalDbContext _localContext;
        private readonly string _filter;
        private readonly WordsType _type;

        public WordsSource(WordsDbContext context, LocalDbContext localContext, WordsType type, string filter)
        {
            _context = context;
            _localContext = localContext;
            _type = type;
            _filter = filter;
        }

        public Task<IEnumerable<WordUi>> GetPagedItemsAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default
        )
        {
            Task.Run(() => { Debug.WriteLine($"GetPagedItems {pageIndex}"); });
            return Task.Run(() =>
            {
                switch (_type)
                {
                    case WordsType.HISTORY:
                        return _localContext.Histories
                            .AsNoTracking()
                            .Where(w => w.Value.StartsWith(_filter))
                            .OrderByDescending(w => w.Id)
                            .Select(w => new WordUi { Id = w.WordId, Value = w.Value })
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .AsEnumerable();
                    case WordsType.BOOKMARK:
                        return _localContext.Bookmarks
                            .AsNoTracking()
                            .Where(w => w.Value.StartsWith(_filter))
                            .OrderByDescending(w => w.Id)
                            .Select(w => new WordUi { Id = w.WordId, Value = w.Value })
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .AsEnumerable();
                    default:
                        return _context.Words
                            .AsNoTracking()
                            .Where(w => w.Value.StartsWith(_filter))
                            .OrderBy(w => w.Value)
                            .Select(w => new WordUi { Id = w.Id, Value = w.Value })
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .AsEnumerable();
                }
            }, cancellationToken);
        }
    }

    public enum WordsType
    {
        HOME,
        HISTORY,
        BOOKMARK
    }
}