using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UwpDictionary.Pages.Words;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UwpDictionary
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
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
						.AddDbContext<LocalDbContext>()
						.AddDbContext<WordsDbContext>()

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
			InitNotificationBackgroundTask();
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
			var hubName = "io.github.sovathna.khmerdictionary";
			var connectionString = "Endpoint=sb://io-github-sovathna-khmerdictionary.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=tvaFPqxshjUYwEjsUa/o2ubTwH/8bIYozlJdGQqJEso=";
			var hub = new NotificationHub(hubName, connectionString);
			Debug.WriteLine(channel.Uri.ToString());
			await hub.RegisterNativeAsync(channel.Uri);
		}



		private async void InitNotificationBackgroundTask()
		{
			var taskName = "UwpDictionary.NotificationTask";
			var isTaskRunning = false;
			foreach (var task in BackgroundTaskRegistration.AllTasks)
			{
				if (task.Value.Name == taskName)
				{
					isTaskRunning = true;
					break;
				}
			}
			if (!isTaskRunning)
			{
				var accessStatus = await BackgroundExecutionManager.RequestAccessAsync();
				if (accessStatus != BackgroundAccessStatus.Unspecified && accessStatus != BackgroundAccessStatus.DeniedByUser && accessStatus != BackgroundAccessStatus.DeniedBySystemPolicy)
				{
					var builder = new BackgroundTaskBuilder();
					var pushTrigger = new PushNotificationTrigger();
					var actionTrigger = new ToastNotificationActionTrigger();
					builder.SetTrigger(pushTrigger);
					builder.SetTrigger(actionTrigger);
					builder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));
					builder.Name = taskName;
					_ = builder.Register();
				}
			}
		}

		protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
		{
			if (args.TaskInstance.TriggerDetails is RawNotification notification)
			{
				new ToastContentBuilder()
					.AddArgument("action", "viewConversation")
					.AddText(notification.Content)
					.AddText("test text")
					.Show();
			}
			else if(args.TaskInstance.TriggerDetails is ToastNotificationActionTriggerDetail detail)
			{
				Debug.WriteLine(detail.Argument);
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
				Debug.WriteLine(tmp.Argument);
			}

			Frame rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (eventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				Window.Current.Content = rootFrame;
			}

			if (eventArgs is LaunchActivatedEventArgs e)
			{
				if (e.PrelaunchActivated == false)
				{
					if (rootFrame.Content == null)
					{
						// When the navigation stack isn't restored navigate to the first page,
						// configuring the new page by passing required information as a navigation
						// parameter
						rootFrame.Navigate(typeof(MainPage), e.Arguments);
					}

					// Ensure the current window is active
					Window.Current.Activate();
				}
			}
			else
			{
				if (rootFrame.Content == null)
				{
					// When the navigation stack isn't restored navigate to the first page,
					// configuring the new page by passing required information as a navigation
					// parameter
					rootFrame.Navigate(typeof(MainPage));
				}

				// Ensure the current window is active
				Window.Current.Activate();
			}
		}
	}
}