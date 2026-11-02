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

		/// <summary>
		/// Get the instance of the logger
		/// </summary>
		/// <param name="loggerName">The value is not used now.</param>
		/// <returns>The value of the logger (singleton)</returns>
		public static IPluginLogger GetLogger(string loggerName)
		{
			return logger;
		}
	}
}
