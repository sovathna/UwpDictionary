﻿
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.EntityFrameworkCore;
using Windows.System;
using System.Diagnostics;
using System.ComponentModel;
using Windows.UI.Xaml.Documents;
using System.Collections.Generic;

namespace UwpDictionary.Pages.Words
{
    public partial class WordsViewModel : ObservableObject, IDisposable
    {
        private readonly CancellationTokenSource _cancelTokenSource;
        private readonly DispatcherQueue _dispatcherQueue;
        private readonly WordsDbContext _context;
        private readonly LocalDbContext _localContext;

        private WordUi _selected;

        public Task<Word> SetSelected(int id)
        {
            return Task.Run(async () =>
            {
                var word = await _context.Words
                .AsNoTracking()
                .Where(w => w.Id == id)
                .FirstOrDefaultAsync();
                _selected = new WordUi { Id = word.Id, Value = word.Value };

                //Check bookmark
                var tmp = await GetBookmark(id);
                ShowBookmarkIcon(tmp != null);

                AddHistory(id);

                return word;
            }, _cancelTokenSource.Token);
        }

        public Visibility BookmarkBtnVisibility =>
            _bookmarkGlyph == null ? Visibility.Collapsed : Visibility.Visible;

        private string _bookmarkGlyph;

        public string BookmarkGlyph
        {
            get => _bookmarkGlyph;
            set
            {
                if (SetProperty(ref _bookmarkGlyph, value))
                {
                    OnPropertyChanged(nameof(BookmarkBtnVisibility));
                }

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

        public void AddHistory(int id)
        {
            Task.Run(async () =>
           {
               if (_selected == null)
                   return;
               var found = await _localContext.Histories
               .Where(w => w.WordId == id)
               .FirstOrDefaultAsync();

               if (found != null)
               {
                   _ = _localContext.Histories.Remove(found);
               }

               var history = new History { WordId = _selected.Id, Value = _selected.Value };
               _ = await _localContext.Histories
               .AddAsync(history);

               _ = await _localContext.SaveChangesAsync();
           }, _cancelTokenSource.Token);
        }

        public Task AddOrDeleteBookmark()
        {
            return Task.Run(async () =>
            {
                if (_selected == null)
                    return;
                var found = await GetBookmark(_selected.Id);
                // If found delete from bookmark, or else add bookmark
                var shouldShowBookmark = found == null;
                ShowBookmarkIcon(shouldShowBookmark);
                if (found != null)
                {
                    _ = _localContext.Bookmarks.Remove(found);
                }
                else
                {
                    var bookmark = new Bookmark { WordId = _selected.Id, Value = _selected.Value };
                    _ = await _localContext.Bookmarks
                    .AddAsync(bookmark);
                }
                _ = await _localContext.SaveChangesAsync();
            }, _cancelTokenSource.Token);
        }



        private IncrementalLoadingCollection<WordsSource, WordUi> _wordCollection;

        public IncrementalLoadingCollection<WordsSource, WordUi> WordCollection
        {
            get => _wordCollection;
            set => SetProperty(ref _wordCollection, value);
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
            TryCreateSearchCalcellationTokenSource();
            Task.Run(async () =>
            {
                await Task.Delay(500, _searchCancelTokenSource.Token);
                if (_lastSearch == filter)
                {
                    return;
                }
                await _dispatcherQueue.EnqueueAsync(() =>
                {
                    var wordsSource = new WordsSource(_context, _localContext, type, filter);
                    WordCollection = new IncrementalLoadingCollection<WordsSource, WordUi>(wordsSource, 100);
                });
                _lastSearch = filter;
            }, _searchCancelTokenSource.Token);
        }

        private void TryCreateSearchCalcellationTokenSource()
        {
            if (_searchCancelTokenSource != null && !_searchCancelTokenSource.IsCancellationRequested)
            {
                _searchCancelTokenSource.Cancel();
            }
            _searchCancelTokenSource = new CancellationTokenSource();
        }

        public Visibility IsLoading(bool isLoading) =>
            isLoading ? Visibility.Visible : Visibility.Collapsed;

        public Visibility EmptyVisibility(int count, bool isLoading) =>
            count == 0 && !isLoading ? Visibility.Visible : Visibility.Collapsed;

        private void ShowBookmarkIcon(bool isBookmark)
        {
            Debug.WriteLine($"isBookmark {isBookmark}");
            _dispatcherQueue.TryEnqueue(() => BookmarkGlyph = isBookmark ? "\uE735" : "\uE734");
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
