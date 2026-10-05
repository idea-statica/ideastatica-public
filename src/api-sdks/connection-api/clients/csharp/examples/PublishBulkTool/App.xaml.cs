using System.Windows;
using System.Windows.Threading;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace PublishBulkTool
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Last resort behind the per-handler try/catch blocks. Without it an exception escaping an
			// 'async void' event handler terminates the process and the only trace left is a Windows
			// Event Log entry - which is exactly how work item 37164 was reported to us.
			DispatcherUnhandledException += OnDispatcherUnhandledException;
			AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Report("Unexpected error", e.Exception);
			e.Handled = true;
		}

		private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			// A non-Exception throw or one from a non-UI thread; the process is going down either way,
			// so all this can do is say what happened before it does.
			Report("Unexpected error", e.ExceptionObject as Exception);
		}

		private static void Report(string title, Exception? exception)
		{
			MessageBox.Show(
				exception?.ToString() ?? "Unknown error.",
				title,
				MessageBoxButton.OK,
				MessageBoxImage.Error);
		}
	}
}
