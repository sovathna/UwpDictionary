using System;
using System.Diagnostics;
using UwpDictionary.Pages.About;
using UwpDictionary.Pages.Settings;
using UwpDictionary.Pages.Words;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpDictionary
{

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly CoreApplicationViewTitleBar CoreTitleBar;
		private readonly SolidColorBrush foregroundBrush;

		public MainPage()
		{
			CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
			CoreTitleBar.ExtendViewIntoTitleBar = true;
			CoreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
			Window.Current.Activated += Window_Activated;

			var titleBar = ApplicationView.GetForCurrentView().TitleBar;
			foregroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
			var b = (SolidColorBrush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
			titleBar.ButtonBackgroundColor = b.Color;
			titleBar.BackgroundColor = b.Color;
			titleBar.ButtonInactiveBackgroundColor = b.Color;
			

			InitializeComponent();

			Window.Current.SetTitleBar(TitleGrid);

			ContentFrame.Navigate(typeof(WordsPage));
			NavView.SelectedItem = NavView.MenuItems[0];
			ContentFrame.Navigated += ContentFrame_Navigated;
		
			SetNavigatedTitle();
		}

		private void Window_Activated(object sender, WindowActivatedEventArgs args)
		{
			UISettings settings = new UISettings();
			if (args.WindowActivationState == CoreWindowActivationState.Deactivated)
			{
				AppTitleTextBlock.Foreground =
					new SolidColorBrush(settings.UIElementColor(UIElementType.GrayText));
			}
			else
			{
				AppTitleTextBlock.Foreground = foregroundBrush;
			}
		}

		private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
		{
			LeftPaddingColumn.Width = new GridLength(CoreTitleBar.SystemOverlayLeftInset);
			RightPaddingColumn.Width = new GridLength(CoreTitleBar.SystemOverlayRightInset);
		}

		private void SetTitle(string title)
		{
			Debug.WriteLine($"Title {title}");
			ApplicationView.GetForCurrentView().Title = title;
			AppTitleTextBlock.Text = title;
		}

		private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
		{
			NavView.IsBackEnabled = ContentFrame.CanGoBack;
			if (ContentFrame.CurrentSourcePageType == typeof(WordsPage))
			{
				NavView.SelectedItem = NavView.MenuItems[0];
			}
			else if (ContentFrame.CurrentSourcePageType == typeof(HistoriesPage))
			{
				NavView.SelectedItem = NavView.MenuItems[1];
			}
			else if (ContentFrame.CurrentSourcePageType == typeof(BookmarksPage))
			{
				NavView.SelectedItem = NavView.MenuItems[2];
			}
			else if (ContentFrame.CurrentSourcePageType == typeof(AboutPage))
			{
				NavView.SelectedItem = NavView.FooterMenuItems[0];
			}
			else if (ContentFrame.CurrentSourcePageType == typeof(SettingsPage))
			{
				NavView.SelectedItem = NavView.FooterMenuItems[1];
			}
			SetNavigatedTitle();
		}

		private void SetNavigatedTitle()
		{
			if (NavView.SelectedItem is Microsoft.UI.Xaml.Controls.NavigationViewItem item)
			{
				SetTitle(item.Content.ToString());
			}
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
				default:
					break;
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
