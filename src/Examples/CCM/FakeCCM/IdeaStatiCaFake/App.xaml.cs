using System;
using System.Diagnostics;
using System.Windows;

namespace IdeaStatiCaFake
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);
		}

		private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
		{
			Exception e = (Exception)args.ExceptionObject;
			Debug.Assert(false, string.Format("{0}\n{1}", e.Message, args.IsTerminating));
		}
	}
}