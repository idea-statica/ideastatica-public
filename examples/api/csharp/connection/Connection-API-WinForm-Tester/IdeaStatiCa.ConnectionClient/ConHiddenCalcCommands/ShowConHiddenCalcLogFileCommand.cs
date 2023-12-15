using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	/// <summary>
	/// Command for opening Idea Connection Hidden calculator log file
	/// </summary>
	public class ShowConHiddenCalcLogFileCommand : ICommand
	{
#pragma warning disable CS0067 // The event is never used
		public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

		public bool CanExecute(object parameter)
		{
			return CanOpenLogFile(parameter);
		}

		public void Execute(object parameter)
		{
			OpenLogFile(parameter);
		}

		/// <summary>
		/// Can CCM log file be opened 
		/// </summary>
		/// <param name="arg"></param>
		/// <returns>Returs true if CCM log file exists</returns>
		public bool CanOpenLogFile(object arg)
		{
			return File.Exists(GetHiddenCalLogFile());
		}

		/// <summary>
		/// Open CCM log file if it exists in the temporary directory
		/// </summary>
		/// <param name="obj"></param>
		public void OpenLogFile(object obj)
		{
			string logFilePath = GetHiddenCalLogFile();

			if (File.Exists(logFilePath))
			{
				using (Process proc = new Process())
				{
					proc.StartInfo = new ProcessStartInfo(logFilePath);
					proc.Start();
				}
			}
		}

		/// <summary>
		/// Returns the full file name of CCM log file
		/// </summary>
		/// <returns></returns>
		private static string GetHiddenCalLogFile()
		{
			var logFileFileName = Path.Combine(Path.GetTempPath(), "IdeaStatiCa\\Logs\\", "IdeaStatiCaConnHiddenCalculator.log");
			return logFileFileName;
		}
	}
}
