using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace UwpDictionary.Pages.Words
{
	public class WordsSource : IIncrementalSource<WordUi>
	{
		private readonly WordsDbContext _context;
		private readonly string _filter;

		public WordsSource(WordsDbContext context, string filter)
		{
			_context = context;
			_filter = filter;
		}

		public Task<IEnumerable<WordUi>> GetPagedItemsAsync(
			int pageIndex, int pageSize,
			CancellationToken cancellationToken = new CancellationToken()
		)
		{
			Task.Run(() =>
			{
				Debug.WriteLine("GetPagedItems {0}", pageIndex);
			});
			return Task.Run(() =>
			{
				return _context.Words
				.Where(w => w.Value.StartsWith(_filter))
				.OrderBy(w => w.Value)
					.Select(w => new WordUi { Id = w.Id, Value = w.Value })
					.Skip(pageIndex * pageSize)
					.Take(pageSize)
					.AsEnumerable();
			}, cancellationToken);
		}
	}
}