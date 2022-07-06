
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml;

namespace UwpDictionary.Pages.Words
{
	public partial class WordsViewModel : ObservableObject
	{
		private readonly WordsDbContext _context;

		private IncrementalLoadingCollection<WordsSource, WordUi> _origin;
		private IncrementalLoadingCollection<WordsSource,WordUi> _wordCollection;

		public IncrementalLoadingCollection<WordsSource,WordUi> WordCollection
		{
			get { return _wordCollection; }
			set { 
				SetProperty(ref _wordCollection, value);
			}
		}

		public WordsViewModel(WordsDbContext context)
		{
			_context = context;
			var wordsSource = new WordsSource(context,"");
			_origin = new IncrementalLoadingCollection<WordsSource, WordUi>(wordsSource, 100);
			WordCollection = _origin;
		}

		public void Search(string filter)
		{
			if (string.IsNullOrWhiteSpace(filter))
			{
				WordCollection = _origin;
			}
			else
			{
				var wordsSource = new WordsSource(_context, filter);
				WordCollection = new IncrementalLoadingCollection<WordsSource, WordUi>(wordsSource, 100);
			}
		
		}
	}
}
