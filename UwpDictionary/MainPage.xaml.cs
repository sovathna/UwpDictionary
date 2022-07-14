using System;
using UwpDictionary.Pages.About;
using UwpDictionary.Pages.Settings;
using UwpDictionary.Pages.Words;
using VungleSDK;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpDictionary
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly VungleAd _vungle;

		public MainPage()
		{
			_vungle = AdFactory.GetInstance("5c91b152974c19001132e471");
			//_vungle.Diagnostic += VungleAd_Diagnostic;

			InitializeComponent();
			ContentFrame.Navigate(typeof(WordsPage));
			NavView.SelectedItem = NavView.MenuItems[0];
            ContentFrame.Navigated += ContentFrame_Navigated;
			//var titleBar = CoreApplication.GetCurrentView().TitleBar;
			//titleBar.ExtendViewIntoTitleBar = true;
		}

        private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
			NavView.IsBackEnabled = ContentFrame.CanGoBack;
            if (ContentFrame.CurrentSourcePageType == typeof(WordsPage))
            {
				NavView.SelectedItem = NavView.MenuItems[0];
			}else if (ContentFrame.CurrentSourcePageType == typeof(HistoriesPage))
			{
				NavView.SelectedItem = NavView.MenuItems[1];
			} else if (ContentFrame.CurrentSourcePageType == typeof(BookmarksPage))
			{
				NavView.SelectedItem = NavView.MenuItems[2];
			}else if (ContentFrame.CurrentSourcePageType == typeof(AboutPage))
			{
				NavView.SelectedItem = NavView.FooterMenuItems[1];
			}else if (ContentFrame.CurrentSourcePageType == typeof(SettingsPage))
			{
				NavView.SelectedItem = NavView.FooterMenuItems[2];
			}
		}

        private void VungleAd_Diagnostic(object sender, DiagnosticLogEvent e)
		{
			System.Diagnostics.Debug.WriteLine("Diagnostic - " + e.Level + " " + e.Type + " " + e.Exception + " " + e.Message);
		}

		private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
		{
			var tag = args.InvokedItemContainer.Tag.ToString();
			switch (tag)
			{
				case "home":
					Navigate(typeof(WordsPage));
					break;
				case "history":
					Navigate(typeof(HistoriesPage));
					break;
				case "bookmark":
					Navigate(typeof(BookmarksPage));
					break;
				case "about":
					Navigate(typeof(AboutPage));
					break;
				case "settings":
					Navigate(typeof(SettingsPage));
					break;
				case "video_ad":
					DispatcherQueue.GetForCurrentThread().TryEnqueue(async () =>
					{
						await _vungle.PlayAdAsync(new AdConfig(), "DEFAULT-9324621");
					});
					break;
				default:
					break;
			}
		}

		private void Navigate(Type pageType)
        {
			if(pageType != ContentFrame.CurrentSourcePageType)
            {
				ContentFrame.Navigate(pageType);
            }
        }

		private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
		{
			if (ContentFrame.CanGoBack)
			{
				ContentFrame.GoBack();
			}
		}
	}
}
