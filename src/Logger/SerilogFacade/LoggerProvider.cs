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

#if DEBUG
			bool isDebug = true;
#else
			bool isDebug = false;
#endif

			logger = new SerilogFacade(SerilogFacade.GetDefaultLogFileName(), isDebug);
		}

		public static IPluginLogger GetLogger(string loggerName)
		{
			return logger;
		}
	}
}
