using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpDictionary.Pages.Words
{
    public sealed class HistoriesPage : AbstractWordsPage
    {
        protected override WordsType Type => WordsType.HISTORY;
    }

    public sealed class BookmarksPage : AbstractWordsPage
    {
        protected override WordsType Type => WordsType.BOOKMARK;
    }

    public sealed class WordsPage : AbstractWordsPage
    {
        protected override WordsType Type => WordsType.HOME;
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public abstract partial class AbstractWordsPage : Page
    {
        private readonly WordsViewModel _viewModel = App.Current.Services.GetRequiredService<WordsViewModel>();
        protected abstract WordsType Type { get; }
        public AbstractWordsPage()
        {
            InitializeComponent();
            Search("");
            Loaded += AbstractWordsPage_Loaded;
        }

        private void AbstractWordsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Type != WordsType.HOME)
            {
                if(_viewModel.WordCollection!=null)
                _viewModel.WordCollection.RefreshAsync();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTextBox = (TextBox)sender;
            Search(searchTextBox.Text);
        }

        private void Search(string filter)
        {
            _viewModel.Search(filter, Type, Dispatcher);

        }

        private WordUi _selectedWord;

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var wordUi = (WordUi)e.ClickedItem;
            _selectedWord = wordUi; 
            SetDef(wordUi.Id);
            _viewModel.AddHistory(wordUi);
        }


        private void HyperLink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Debug.WriteLine(sender.Name);
            SetDef(int.Parse(sender.Name));
        }

        private void SetDef(int id)
        {
            var word = _viewModel.GetWord(id);
            WordTextBlock.Text = word.Value;
            DefTextBlock.Blocks.Clear();
            foreach (var s in word.Definition.Split("[NewLine]"))
            {
                var paragraph = new Paragraph();

                paragraph.Margin = new Thickness
                {
                    Bottom = 32
                };
                foreach (var s1 in s.Split("[]"))
                {
                    if (s1.Contains('|'))
                    {
                        var tmps = s1.Split("|");
                        var run = new Run();
                        run.Text = tmps[1];
                        var hyperLink = new Hyperlink()
                        {
                            UnderlineStyle = UnderlineStyle.None,
                        };
                        hyperLink.SetValue(NameProperty, tmps[0]);
                        hyperLink.Click += HyperLink_Click; ;
                        hyperLink.Foreground = new SolidColorBrush(Colors.Black);

                        hyperLink.Inlines.Add(run);
                        paragraph.Inlines.Add(hyperLink);
                    }
                    else
                    {
                        var run = new Run();
                        if (s1.Contains("[HI]"))
                        {
                            run.Text = s1.Replace("[HI]", "");
                            run.Foreground = new SolidColorBrush(Colors.PaleVioletRed);
                        }
                        else if (s1.Contains("[HI1]"))
                        {
                            run.Text = s1.Replace("[HI1]", "");
                            run.Foreground = new SolidColorBrush(Colors.DarkRed);
                        }
                        else
                        {
                            run.Text = s1;
                            run.Foreground = new SolidColorBrush(Colors.Black);
                        }

                        paragraph.Inlines.Add(run);
                    }
                }
                DefTextBlock.Blocks.Add(paragraph);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddBookmark(_selectedWord);
        }
    }
}
