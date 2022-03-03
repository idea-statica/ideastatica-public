using IdeaStatiCa.Plugin;
using Serilog;
using System;
using System.IO;

namespace IdeaStatiCa.PluginLogger
{
	/// <summary>
	/// Allows to use Serilog as IPluginLogger
	/// </summary>
	public class SerilogFacade : IPluginLogger, IDisposable
	{
		// format used for logging to the Serilog
		private static readonly string SerilogFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({ProcessId}-{ThreadId}) {Message:lj}{NewLine}{Exception}";


		// global Serilog logger (Serilog allows to create only one logger, so we are creating is one and share it across all our IdeaLoggers)
		private Serilog.Core.Logger globalSerilogLogger = null;
		private bool disposedValue;

		/// <summary>
		/// Returns the default logfilename for IdeaRstabPlugin
		/// If the directory for logfile doesn't exist it will be created
		/// </summary>
		/// <exception cref="System.Exception">Exception can be thrown if directory for logfile can not be created</exception>
		/// <returns>Valid file name for log file</returns>
		public static string GetDefaultLogFileName()
		{
			var defaultLogDir = Path.Combine(Path.GetTempPath(), "IdeaStatiCa\\Logs");

			// create directory, of needed
			if (!Directory.Exists(defaultLogDir))
			{
				Directory.CreateDirectory(defaultLogDir);
			}

			// determine the log file path
			var logPath = Path.Combine(defaultLogDir, "IdeaRstabPlugin.log");
			return logPath;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="logFileName"></param>
		/// <param name="logToDebugView"></param>
		/// <param name="logMinLevel"></param>
		public SerilogFacade(string logFileName, bool logToDebugView)
		{
			// create the default logger configuration
			LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
				// enrich the logs with process and thread id
#if DEBUG
				.MinimumLevel.Debug()
#else
				.MinimumLevel.Information()
#endif
				.Enrich.WithProcessId()
				.Enrich.WithThreadId();

			if(!string.IsNullOrEmpty(logFileName))
			{
				// default log size is 512 kB
				long DefaultFileSize = 512 * 1024;

				// activate the rolling file logging with 4 512kB log files
				loggerConfiguration = loggerConfiguration.WriteTo.File(
					path: logFileName,
					outputTemplate: SerilogFormat,
					rollOnFileSizeLimit: true,
					fileSizeLimitBytes: DefaultFileSize,
					retainedFileCountLimit: 4,
					shared: true);
			}

			// if the logging to the DebugView should be active
			if (logToDebugView)
			{
				// activate it
				loggerConfiguration = loggerConfiguration.WriteTo.Debug(outputTemplate: SerilogFormat);
			}

			// create the global serilog logger
			globalSerilogLogger = loggerConfiguration.CreateLogger();

		}

		/// <inheritdoc cref="IPluginLogger.LogDebug(string, Exception)"/>
		public void LogDebug(string message, Exception ex = null)
		{
			globalSerilogLogger.Debug(message);
		}

		/// <inheritdoc cref="IPluginLogger.LogError(string, Exception)"/>
		public void LogError(string message, Exception ex = null)
		{
			globalSerilogLogger.Error(message);
		}

		/// <inheritdoc cref="IPluginLogger.LogInformation(string, Exception)"/>
		public void LogInformation(string message, Exception ex = null)
		{
			globalSerilogLogger.Information(message);
		}

		/// <inheritdoc cref="IPluginLogger.LogTrace(string, Exception)"/>
		public void LogTrace(string message, Exception ex = null)
		{
			globalSerilogLogger.Verbose(message);
		}

		/// <inheritdoc cref="IPluginLogger.LogWarning(string, Exception)"/>
		public void LogWarning(string message, Exception ex = null)
		{
			globalSerilogLogger.Warning(message);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if(globalSerilogLogger != null)
					{
						globalSerilogLogger.Dispose();
						globalSerilogLogger = null;
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~SerilogFacade()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
