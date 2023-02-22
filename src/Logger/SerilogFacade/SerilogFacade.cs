using IdeaStatiCa.Plugin;
using IdeaStatiCa.Public;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace IdeaStatiCa.PluginLogger
{
	/// <summary>
	/// Allows to use Serilog as IPluginLogger
	/// </summary>
	public class SerilogFacade : IPluginLogger, IDisposable
	{
		// format used for logging to the Serilog
		private static readonly string SerilogFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({ProcessId}-{ThreadId}) {Message:lj}{NewLine}{Exception}";

		/// <summary>
		/// File name of the log file
		/// </summary>
		private static string LogFileName { get; set; }

		// global Serilog logger (Serilog allows to create only one logger, so we are creating is one and share it across all our IdeaLoggers)
		private Serilog.Core.Logger globalSerilogLogger = null;

		private bool disposedValue;
		public static string LogLevel { get; private set; }

		static SerilogFacade()
		{
			LogLevel = "Information";
			LogFileName = GetLogName() + ".log";

			// try to get log level from IdeaDiagnostics.config
			// load the XML

			string userProfileLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string ideaRsPath = Path.Combine(userProfileLocalPath, "IDEA_RS");
			string diagnosticsConfFile = Path.Combine(ideaRsPath, "IdeaDiagnostics.config");

			if (File.Exists(diagnosticsConfFile))
			{
				XDocument doc = XDocument.Load(diagnosticsConfFile);

				// get all the elements under <IdeaDiagnosticsSettings> tag
				var xmlElements = doc.Descendants("IdeaDiagnosticsSettings").Nodes().Where(x => x.NodeType == System.Xml.XmlNodeType.Element).Cast<XElement>();

				// get the default log level value from the <DefaultLogLevel> tag
				var defaultLogLevelTag = xmlElements.Where(x => x.Name.ToString() == "DefaultLogLevel").FirstOrDefault();
				// if the tag was found
				if (defaultLogLevelTag != null)
				{
					LogLevel = defaultLogLevelTag.Attribute("loglevel").Value.ToString();
				}
			}
		}

		/// <summary>
		/// Initialization of plugin logger must be the first call
		/// </summary>
		/// <param name="logFileName"></param>
		public static void Initialize(string logFileName = null)
		{
			if (!string.IsNullOrEmpty(logFileName))
			{
				LogFileName = logFileName;
			}
		}

		/// <summary>
		/// Returns the default full pathname for log file
		/// If the directory for logfile doesn't exist it will be created.
		/// Log file can be found in the directory  '%Temp%\IdeaStatiCa\Logs\'
		/// The logfilename corresponds to the name of the entry assembly but it can be set by calling the method Initialze
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
			var logPath = Path.Combine(defaultLogDir, LogFileName);
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
				.Enrich.WithProcessId()
				.Enrich.WithThreadId();

			var logLevel = LogLevel.Trim();
			logLevel = logLevel.ToLowerInvariant();

			switch (logLevel)
			{
				case "debug":
					{
						loggerConfiguration = loggerConfiguration.MinimumLevel.Debug();
						break;
					}

				case "trace":
					{
						loggerConfiguration = loggerConfiguration.MinimumLevel.Verbose();
						break;
					}

				case "warning":
					{
						loggerConfiguration = loggerConfiguration.MinimumLevel.Warning();
						break;
					}

				case "error":
					{
						loggerConfiguration = loggerConfiguration.MinimumLevel.Error();
						break;
					}

				case "critical":
					{
						loggerConfiguration = loggerConfiguration.MinimumLevel.Error();
						break;
					}

				default:
					loggerConfiguration = loggerConfiguration.MinimumLevel.Information();
					break;
			}

			if (!string.IsNullOrEmpty(logFileName))
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
					if (globalSerilogLogger != null)
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

		private static string GetLogName()
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();

			if (entryAssembly != null)
			{
				AssemblyName entryAssemblyName = entryAssembly.GetName();
				return entryAssemblyName.Name;
			}

			Process process = Process.GetCurrentProcess();
			return $"{process.ProcessName}_{process.Id}";
		}

		public void LogEventInformation(IIdeaUserEvent userEvent, string screenName = null, Dictionary<int, string> eventCustomDimensions = null)
		{
		}
	}
}