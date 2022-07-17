using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpDictionary.Pages.Settings
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsPage : Page
	{

		private readonly ApplicationDataContainer _settings;

		public SettingsPage()
		{
			InitializeComponent();

			_settings = ApplicationData.Current.LocalSettings;
			var tag = _settings.Values["SettingsTheme"] ?? "RadioThemeSystem";
			switch (tag)
			{
				case "RadioThemeSystem":
					RadioThemeSystem.IsChecked = true;
					break;
				case "RadioThemeLight":
					RadioThemeLight.IsChecked = true;
					break;
				case "RadioThemeDark":
					RadioThemeDark.IsChecked = true;
					break;
			}
			var textSize = (double?)_settings.Values["SettingsTextSize"] ?? 14;
			SliderTextSize.ValueChanged += Slider_ValueChanged;
			SliderTextSize.Value = textSize;
		}

		private void RadioButton_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			var radio = (RadioButton)sender;
			switch (radio.Tag.ToString())
			{
				case "RadioThemeSystem":
					if (App.Current.SystemTheme == ApplicationTheme.Light)
					{
						((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
					}
					else
					{
						((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
					}
					break;
				case "RadioThemeLight":
					((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
					break;
				case "RadioThemeDark":
					((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
					break;
			}
			if (_settings != null)
			{
				_settings.Values["SettingsTheme"] = radio.Tag.ToString();
			}
		}

		private void Slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
		{
			var slider = (Slider)sender;
			_settings.Values["SettingsTextSize"] = slider.Value;
			TextBlockTextSize.FontSize = slider.Value;
		}
	}
}