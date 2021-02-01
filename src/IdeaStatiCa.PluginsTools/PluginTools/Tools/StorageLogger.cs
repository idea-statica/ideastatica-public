using log4net;

namespace CI.DataModel
{
	/// <summary>
	/// Logging events of Container storage
	/// </summary>
	internal class StorageLogger
	{
		private static readonly ILog logger = LogManager.GetLogger("ContainerStorage");

		/// <summary>
		/// Main application logger
		/// </summary>
		public static readonly ILog AppLogger = LogManager.GetLogger(IdeaRS.Constants.AppLogger);

		public static ILog GetLogger()
		{
			return logger;
		}
	}
}
