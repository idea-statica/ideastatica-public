using Serilog.Configuration;
using Serilog.Core;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.PluginLogger
{
	/// <summary>
	/// Singleton which provides logger to plugin
	/// </summary>
	public class LoggerProvider
	{
		static IPluginLogger logger;

		static LoggerProvider()
		{
			//logger = new NullLogger();
			logger = new SerilogFacade(SerilogFacade.GetDefaultLogFileName(), true);

		}

		public static IPluginLogger GetLogger(string loggerName)
		{
			return logger;
		}
	}
}
