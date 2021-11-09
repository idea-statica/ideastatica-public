using IdeaStatiCa.Plugin;

namespace IdeaRstabPlugin
{
	/// <summary>
	/// Singleton which provides logger to plugin
	/// </summary>
	public class LoggerProvider
	{
		static IPluginLogger logger;

		static LoggerProvider()
		{
			logger = new NullLogger();
		}

		public static IPluginLogger GetLogger(string loggerName)
		{
			return logger;
		}
	}
}
