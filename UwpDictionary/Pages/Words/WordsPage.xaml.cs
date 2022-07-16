using System.Diagnostics;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Microsoft.Extensions.DependencyInjection;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpDictionary.Pages.Words
{
	public sealed class HistoriesPage : AbstractWordsPage
	{
		public HistoriesPage() : base(WordsType.HISTORY)
		{
		}
	}

	public sealed class BookmarksPage : AbstractWordsPage
	{
		public BookmarksPage() : base(WordsType.BOOKMARK)
		{
		}
	}

	public sealed class WordsPage : AbstractWordsPage
	{
		public WordsPage() : base(WordsType.HOME)
		{
		}
	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public abstract partial class AbstractWordsPage : Page
	{
		private readonly WordsViewModel _viewModel = App.Current.Services.GetRequiredService<WordsViewModel>();
		private readonly WordsType _type;
		private readonly ApplicationDataContainer _settings = ApplicationData.Current.LocalSettings;

		protected AbstractWordsPage(WordsType type)
		{
			_type = type;
			InitializeComponent();
			_viewModel.Search("", type);
			Loaded += AbstractWordsPage_Loaded;
		}


		private void AbstractWordsPage_Loaded(object sender, RoutedEventArgs e)
		{
			var size = (double?)_settings.Values["SettingsTextSize"] ?? 14;
			if (size != DefTextBlock.FontSize)
			{
				DefTextBlock.FontSize = (double?)_settings.Values["SettingsTextSize"] ?? 14;
			}

			_viewModel.RefreshSelected(word =>
			{
				WordTextBlock.Text = word.Value;
				DefTextBlock.Blocks.Clear();
				SetDefinition(word);
			});

			if (_type != WordsType.HOME)
				_viewModel.WordCollection?.RefreshAsync();
		}

		private void ListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var wordUi = (WordUi)e.ClickedItem;
			SelectItem(wordUi.Id);
		}

		private void SelectItem(int id)
		{
			_viewModel.SetSelected(id, word =>
			{
				WordTextBlock.Text = word.Value;
				DefTextBlock.Blocks.Clear();
				SetDefinition(word);
			});
		}

		private void SetDefinition(Word word)
		{
			foreach (var s in word.Definition.Replace("ឧទាហរណ៍", "ឧ.").Split("[NewLine]"))
			{
				var paragraph = new Paragraph
				{
					Margin = new Thickness
					{
						Bottom = 32
					}
				};

				foreach (var s1 in s.Split("[]"))
					if (s1.Contains('|'))
					{
						var tmps = s1.Split("|");
						var run = new Run
						{
							Text = tmps[1]
						};
						var hyperLink = new Hyperlink()
						{
							UnderlineStyle = UnderlineStyle.None
						};
						hyperLink.SetValue(NameProperty, tmps[0]);
						hyperLink.Click += HyperLink_Click;

						hyperLink.Foreground = WordTextBlock.Foreground;

						hyperLink.Inlines.Add(run);
						paragraph.Inlines.Add(hyperLink);
					}
					else
					{
						var run = new Run();
						if (s1.Contains("[HI]"))
						{
							run.Text = s1.Replace("[HI]", "");
							run.Foreground = WordTextBlock.Foreground;
						}
						else if (s1.Contains("[HI1]"))
						{
							run.Text = s1.Replace("[HI1]", "");
							run.Foreground = new SolidColorBrush(Colors.Red);
						}
						else
						{
							run.Text = s1;
							run.Foreground = WordTextBlock.Foreground;
						}

						paragraph.Inlines.Add(run);
					}

				DefTextBlock.Blocks.Add(paragraph);
			}
		}


		private void HyperLink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
		{
			WordListView.SelectedItem = null;
			Debug.WriteLine(sender.Name);
			SelectItem(int.Parse(sender.Name));
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.AddOrDeleteBookmark();
		}

		private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var searchTextBox = (TextBox)sender;
			_viewModel.Search(searchTextBox.Text, _type);
		}
	}
}