using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;

namespace UwpDictionary.Pages.Words
{
	public class WordsViewModel : ObservableObject, IDisposable
	{
		private readonly CancellationTokenSource _cancelTokenSource;
		private readonly DispatcherQueue _dispatcherQueue;
		private readonly WordsDbContext _context;
		private readonly LocalDbContext _localContext;

		private WordUi _selected;

		public void RefreshSelected(Action<Word> onResult)
		{
			if (_selected == null)
				return;
			SetSelected(_selected.Id, false, onResult);
		}

		public void SetSelected(int id, bool shouldAddHistory, Action<Word> onResult)
		{
			Task.Run(async () =>
			{
				var word = await _context.Words
					.AsNoTracking()
					.Where(w => w.Id == id)
					.FirstOrDefaultAsync();
				_dispatcherQueue.TryEnqueue(() => { onResult(word); });
				_selected = new WordUi { Id = word.Id, Value = word.Value };

				//Check bookmark
				var tmp = await GetBookmark(id);
				ShowBookmarkIcon(tmp != null);
				if (shouldAddHistory)
					AddHistory(_selected, id);
			}, _cancelTokenSource.Token);
		}

		public Visibility BookmarkBtnVisibility =>
			_selected == null ? Visibility.Collapsed : Visibility.Visible;

		private string _bookmarkIcon = "ms-appx:///Assets/Icons/bookmark_border.png";

		public string BookmarkIcon
		{
			get => _bookmarkIcon;
			private set
			{
				SetProperty(ref _bookmarkIcon, value);
				OnPropertyChanged(nameof(BookmarkBtnVisibility));
			}
		}

		private Task<Bookmark> GetBookmark(int id)
		{
			return Task.Run(async () =>
			{
				var found = await _localContext.Bookmarks
					.Where(w => w.WordId == id)
					.FirstOrDefaultAsync();
				return found;
			}, _cancelTokenSource.Token);
		}

		private void AddHistory(WordUi word, int id)
		{
			Task.Run(async () =>
			{
				if (word == null)
					return;
				var found = await _localContext.Histories
					.Where(w => w.WordId == id)
					.FirstOrDefaultAsync();

				if (found != null)
					_ = _localContext.Histories.Remove(found);

				_ = await _localContext.Histories.AddAsync(_selected.ToHistory());

				_ = await _localContext.SaveChangesAsync();
			}, _cancelTokenSource.Token);
		}

		public void AddOrDeleteBookmark()
		{
			Task.Run(async () =>
			{
				if (_selected == null)
					return;
				var found = await GetBookmark(_selected.Id);
				// If found delete from bookmark, or else add bookmark
				var shouldShowBookmark = found == null;
				ShowBookmarkIcon(shouldShowBookmark);
				if (found != null)
					_ = _localContext.Bookmarks.Remove(found);
				else
					_ = await _localContext.Bookmarks.AddAsync(_selected.ToBookmark());

				_ = await _localContext.SaveChangesAsync();
			}, _cancelTokenSource.Token);
		}


		private IncrementalLoadingCollection<WordsSource, WordUi> _wordCollection;

		public IncrementalLoadingCollection<WordsSource, WordUi> WordCollection
		{
			get => _wordCollection;
			private set => SetProperty(ref _wordCollection, value);
		}

		private string _lastSearch;

		public WordsViewModel(
			DispatcherQueue dispatcherQueue,
			WordsDbContext context,
			LocalDbContext localContext
		)
		{
			_dispatcherQueue = dispatcherQueue;
			_context = context;
			_localContext = localContext;
			_cancelTokenSource = new CancellationTokenSource();
		}

		private CancellationTokenSource _searchCancelTokenSource;

		public void Search(string filter, WordsType type)
		{
			TryCreateSearchCancellationTokenSource();
			Task.Run(async () =>
			{
				await Task.Delay(500, _searchCancelTokenSource.Token);
				if (_lastSearch == filter)
					return;

				var res = _dispatcherQueue.TryEnqueue(() =>
				{
					var wordsSource = new WordsSource(_context, _localContext, type, filter);
					WordCollection = new IncrementalLoadingCollection<WordsSource, WordUi>(wordsSource, 100);
				});
				if (res)
					_lastSearch = filter;
			}, _searchCancelTokenSource.Token);
		}

		private void TryCreateSearchCancellationTokenSource()
		{
			if (_searchCancelTokenSource != null && !_searchCancelTokenSource.IsCancellationRequested)
				_searchCancelTokenSource.Cancel();
			_searchCancelTokenSource = new CancellationTokenSource();
		}

		public Visibility IsLoading(bool isLoading)
		{
			return isLoading ? Visibility.Visible : Visibility.Collapsed;
		}

		public Visibility EmptyVisibility(int count, bool isLoading)
		{
			return count == 0 && !isLoading ? Visibility.Visible : Visibility.Collapsed;
		}

		private void ShowBookmarkIcon(bool isBookmark)
		{
			_dispatcherQueue.TryEnqueue(() =>
				BookmarkIcon = isBookmark
					? "ms-appx:///Assets/Icons/bookmark.png"
					: "ms-appx:///Assets/Icons/bookmark_border.png");
		}

		public void Dispose()
		{
			_searchCancelTokenSource.Dispose();
			_cancelTokenSource.Dispose();
			_context.DisposeAsync();
			_localContext.DisposeAsync();
		}
	}
}