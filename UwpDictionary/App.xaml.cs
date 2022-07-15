using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAzure.Messaging;
using System;
using System.Diagnostics;
using UwpDictionary.Pages.Words;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UwpDictionary
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		public new static App Current => (App)Application.Current;

		public IServiceProvider Services { get; }

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			var host = new HostBuilder()
				.ConfigureServices(
					(_, services) => services
						.AddSingleton(DispatcherQueue.GetForCurrentThread())

						.AddDbContextPool<WordsDbContext>(options =>
						{
							var dbPath = Package.Current.InstalledPath + "/Assets/Databases/khdict.sqlite";
							var connectionString = "Data Source=" + dbPath;
							options
								.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
								.EnableSensitiveDataLogging()
								.EnableDetailedErrors()
								.UseSqlite(connectionString);
						})
						.AddDbContextPool<LocalDbContext>(options =>
						{
							var dbPath = ApplicationData.Current.LocalFolder.Path + "/local.sqlite";
							var connectionString = "Data Source=" + dbPath;
							options
								.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
								.EnableSensitiveDataLogging()
								.EnableDetailedErrors()
								.UseSqlite(connectionString);
						})
						.AddDbContext<LocalDbContext>()

						.AddTransient<WordsViewModel>()
				)
				.Build();
			host.RunAsync();
			Services = host.Services;

			InitializeComponent();
			Suspending += OnSuspending;
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			InitNotificationsAsync();
			OnLaunchedOrActivated(e);
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}

		private async void InitNotificationsAsync()
		{
			var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
			Debug.WriteLine(channel.Uri.ToString());
		}

		protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
		{
			if (args.TaskInstance.TriggerDetails is ToastNotificationActionTriggerDetail detail)
			{
				Debug.WriteLine($"OnBackgroundActivated {detail.Argument}");
			}
		}

		protected override void OnActivated(IActivatedEventArgs e)
		{
			OnLaunchedOrActivated(e);
		}

		private void OnLaunchedOrActivated(IActivatedEventArgs eventArgs)
		{
			if (eventArgs is ToastNotificationActivatedEventArgs tmp)
			{
				var args = ToastArguments.Parse(tmp.Argument);
				Debug.WriteLine($"OnLaunchedOrActivated {tmp.Argument}");
			}

			var rootFrame = (Frame)Window.Current.Content;

			if (rootFrame == null)
			{
				rootFrame = new Frame();
				rootFrame.Background = new SolidColorBrush(Colors.Magenta);

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (eventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}
				Window.Current.Content = rootFrame;
			}

			if (eventArgs is LaunchActivatedEventArgs e)
			{
				if (e.PrelaunchActivated == false)
				{
					TryEnablePrelaunch();
					if (rootFrame.Content == null)
					{
						rootFrame.Navigate(typeof(MainPage), e.Arguments);
					}
					Window.Current.Activate();
				}
			}
			else
			{
				if (rootFrame.Content == null)
				{
					rootFrame.Navigate(typeof(MainPage));
				}
				Window.Current.Activate();
			}
		}

		private void TryEnablePrelaunch()
		{
			bool canEnablePrelaunch = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");
			if (canEnablePrelaunch)
			{
				Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
			}
		}
	}
}