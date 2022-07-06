using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpDictionary.Pages.Words
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class WordsPage : Page
	{
		private readonly WordsViewModel _viewModel = App.Current.Services.GetRequiredService<WordsViewModel>();
		public WordsPage()
		{
			InitializeComponent();
		}

		private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var tb = (TextBox)sender;
			Debug.WriteLine($"Search {tb.Text}");
			_viewModel.Search(tb.Text);
		}
	}
}
