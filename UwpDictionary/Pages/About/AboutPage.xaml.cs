using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpDictionary.Pages.About
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AboutPage : Page
	{
		public AboutPage()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			var button = (Button)sender;
			switch (button.Tag.ToString())
			{
				case "ButtonDeveloper":
					_ = Windows.System.Launcher.LaunchUriAsync(new System.Uri(@"https://sovathna.github.io/"));
					break;
				case "ButtonSourceCode":
					_ = Windows.System.Launcher.LaunchUriAsync(new System.Uri(@"https://github.com/sovathna/UwpDictionary/"));
					break;
				case "ButtonStore":
					_ = Windows.System.Launcher.LaunchUriAsync(new System.Uri(@"ms-windows-store://pdp/?productid=9MZF2G4R2HVB"));
					break;
			}
		}
	}
}