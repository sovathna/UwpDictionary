﻿using Windows.UI.Xaml.Controls;

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
			this.InitializeComponent();
		}

		private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			var button = (Button)sender;
			if (button.Tag.ToString() == "ButtonDeveloper")
			{
				_ = Windows.System.Launcher.LaunchUriAsync(new System.Uri(@"https://sovathna.github.io/"));
			}
			else if (button.Tag.ToString() == "ButtonSourceCode")
			{
				_ = Windows.System.Launcher.LaunchUriAsync(new System.Uri(@"https://github.com/sovathna/Khmer-Dictionary/"));
			}
		}
	}
}
