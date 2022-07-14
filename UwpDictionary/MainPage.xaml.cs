using CommunityToolkit.Mvvm.ComponentModel;
using System;
using UwpDictionary.Pages.About;
using UwpDictionary.Pages.Settings;
using UwpDictionary.Pages.Words;
using VungleSDK;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpDictionary
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly VungleAd _vungle;
		private CoreApplicationViewTitleBar _titleBar;
		private const string VUNGLE_APP_ID = "5c91b152974c19001132e471";
		private const string VUNGLE_REWARDED = "DEFAULT-9324621";
		private const string VUNGLE_BANNER = "BANNER-4643114";

		public MainPage()
		{
			InitializeComponent();
			_vungle = AdFactory.GetInstance(VUNGLE_APP_ID);
			//_vungle.Diagnostic += VungleAd_Diagnostic;
			_vungle.OnAdStart += _vungle_OnAdStart;
			_vungle.OnAdEnd += _vungle_OnAdEnd;
			_vungle.OnAdPlayableChanged += _vungle_OnAdPlayableChanged;


			ContentFrame.Navigate(typeof(WordsPage));
			NavView.SelectedItem = NavView.MenuItems[0];
			ContentFrame.Navigated += ContentFrame_Navigated;
			_titleBar = CoreApplication.GetCurrentView().TitleBar;
			_titleBar.ExtendViewIntoTitleBar = true;
			
			Window.Current.SetTitleBar(TitleGrid);

			SetTitle("វចនានុក្រមខ្មែរ");
		}

		private void SetTitle(string title)
		{
			ApplicationView.GetForCurrentView().Title = title;
			AppTitleTextBlock.Text = title;
		}

		private void _vungle_OnAdEnd(object sender, AdEndEventArgs e)
		{
			if (e.Placement == VUNGLE_REWARDED)
			{
				_ = Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
				{
					_titleBar.ExtendViewIntoTitleBar = true;
				});
			}
		}

		private void _vungle_OnAdStart(object sender, AdEventArgs e)
		{
			if (e.Placement == VUNGLE_REWARDED)
			{
				_ = Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
				{
					_titleBar.ExtendViewIntoTitleBar = false;
				});
			}
		}

		private void _vungle_OnAdPlayableChanged(object sender, AdPlayableEventArgs e)
		{
			_ = Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				if (e.Placement == VUNGLE_REWARDED)
				{
					VideoAdNavItem.IsEnabled = e.AdPlayable;
				}
				else if (e.Placement == VUNGLE_BANNER)
				{
					AdPanel.Visibility = e.AdPlayable ? Visibility.Visible : Visibility.Collapsed;
				}
			});



		}

		private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			var title = "Khmer Dictionary";
			NavView.IsBackEnabled = ContentFrame.CanGoBack;
			if (ContentFrame.CurrentSourcePageType == typeof(WordsPage))
			{
				title = "វចនានុក្រមខ្មែរ";
				NavView.SelectedItem = NavView.MenuItems[0];
			}
			else if (ContentFrame.CurrentSourcePageType == typeof(HistoriesPage))
			{
				title = "បញ្ជីពាក្យធ្លាប់មើល";
				NavView.SelectedItem = NavView.MenuItems[1];
			}
			else if (ContentFrame.CurrentSourcePageType == typeof(BookmarksPage))
			{
				title = "បញ្ជីពាក្យចំណាំ";
				NavView.SelectedItem = NavView.MenuItems[2];
			}
			else if (ContentFrame.CurrentSourcePageType == typeof(AboutPage))
			{
				title = "អំពីកម្មវិធី";
				NavView.SelectedItem = NavView.FooterMenuItems[1];
			}
			else if (ContentFrame.CurrentSourcePageType == typeof(SettingsPage))
			{
				title = "ការកំណត់";
				NavView.SelectedItem = NavView.FooterMenuItems[2];
			}

				SetTitle(title) ;
			
			
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
					TryPlayRewardedAd();
					break;
				default:
					break;
			}
		}

		private void TryPlayRewardedAd()
		{
			if (_vungle.IsAdPlayable(VUNGLE_REWARDED))
			{
				DispatcherQueue.GetForCurrentThread().TryEnqueue(async () =>
				{
					await _vungle.PlayAdAsync(new AdConfig(), VUNGLE_REWARDED);
				});
			}
		}

		private void Navigate(Type pageType)
		{
			if (pageType != ContentFrame.CurrentSourcePageType)
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
