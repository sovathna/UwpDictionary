
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace UwpDictionary.Pages.Words
{
    public partial class WordsViewModel : ObservableObject
    {

        private readonly WordsDbContext _context;
        private readonly LocalDbContext _localContext;

        private IncrementalLoadingCollection<WordsSource, WordUi> _origin;
        private IncrementalLoadingCollection<WordsSource, WordUi> _wordCollection;

        public IncrementalLoadingCollection<WordsSource, WordUi> WordCollection
        {
            get => _wordCollection;
            set
            {
                if (SetProperty(ref _wordCollection, value))
                {
                    OnPropertyChanged(nameof(EmptyVisibility));
                }
            }
        }

        public WordsViewModel(WordsDbContext context, LocalDbContext localContext)
        {
            _context = context;
            _localContext = localContext;
        }

        private CancellationTokenSource _searchCancelToken = new CancellationTokenSource();

        public void Search(string filter, WordsType type, CoreDispatcher dispatcher)
        {
            if (!_searchCancelToken.IsCancellationRequested)
            {
                _searchCancelToken.Cancel();
            }
            _searchCancelToken = new CancellationTokenSource();

            Task.Run(async () =>
           {
               await Task.Delay(600, _searchCancelToken.Token);
               await dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, () =>
               {
                   var wordsSource = new WordsSource(_context, _localContext, type, filter);
                   WordCollection = new IncrementalLoadingCollection<WordsSource, WordUi>(wordsSource, 100);
               });
           }, _searchCancelToken.Token);


        }

        public Visibility IsLoading(bool isLoading) => isLoading ? Visibility.Visible : Visibility.Collapsed;

        public Visibility EmptyVisibility(int count, bool isLoading) => count == 0 && !isLoading ? Visibility.Visible : Visibility.Collapsed;


        public Word GetWord(int id) => _context.Words.AsNoTracking().Where(w => w.Id == id).First();

        public void AddBookmark(WordUi word)
        {
            if (word == null) return;
            var found = _localContext.Bookmarks.Where(w => w.WordId == word.Id).FirstOrDefault();
            if(found != null)
            {
                _localContext.Bookmarks.Remove(found);
            }
            _localContext.Bookmarks.Add(new Bookmark { WordId = word.Id,Value=word.Value});
            _localContext.SaveChangesAsync();
        }
        public void AddHistory(WordUi word)
        {
            if (word == null) return;
            var found = _localContext.Histories.Where(w => w.WordId == word.Id).FirstOrDefault();
            if (found != null)
            {
                _localContext.Histories.Remove(found);
            }
            _localContext.Histories.Add(new History { WordId = word.Id, Value = word.Value });
            _localContext.SaveChangesAsync();
        }
    }
}
