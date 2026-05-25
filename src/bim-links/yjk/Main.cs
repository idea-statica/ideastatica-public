using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace yjk
{
	public class Main
	{
		[CommandMethod("idea_statica")]
		public void Run()
		{
			Dispatcher yjkDispatcher = Dispatcher.CurrentDispatcher;
			AssemblyResolver.Install();

			Task.Run(() =>
			{
				try
				{
					RunPlugin(yjkDispatcher);
				}
				catch (Exception)
				{
				}
			});
		}

		private static void RunPlugin(Dispatcher yjkDispatcher)
		{
			string workingDirectory = Directory.GetCurrentDirectory();
			var yjkFiles = Directory.GetFiles(workingDirectory, "*.yjk");
			string yjkFileName = yjkFiles.Length == 1
				? Path.GetFileNameWithoutExtension(yjkFiles[0]) : "";
			string fullWorkingDirectory = Path.Combine(workingDirectory, "IdeaStatiCa-" + yjkFileName);

			Assembly plugin = Assembly.LoadFrom(Path.Combine(AssemblyResolver.Net48Path, "YjkPlugin.dll"));
			Type entryType = plugin.GetType("yjk.PluginEntry");

			entryType.GetProperty("YjkDispatcher").SetValue(null, yjkDispatcher);
			entryType.GetMethod("Run", new[] { typeof(string) }).Invoke(null, new object[] { fullWorkingDirectory });
		}
	}
}
