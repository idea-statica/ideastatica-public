using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaRS.OpenModel.Message
{
	/// <summary>
	/// Data for progress report of IOM
	/// </summary>
	public class IDEAProgressReport
	{
		/// <summary>
		/// current progress
		/// </summary>
		public int CurrentProgressAmount { get; set; }

		/// <summary>
		/// total progress
		/// </summary>
		public int TotalProgressAmount { get; set; }
		/// <summary>
		/// some message to pass to the UI of current progress
		/// </summary>
		public string CurrentProgressMessage { get; set; }
	}

	/// <summary>
	/// Progress Report for IOM
	/// </summary>
	public class IDEAProgress
	{
		/// <summary>
		/// Callback function of progress
		/// </summary>
		Action<IDEAProgressReport> callback;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="callback">Callback function of progress</param>
		public IDEAProgress(Action<IDEAProgressReport> callback)
		{
			this.callback = callback;
		}

		/// <summary>
		/// Report
		/// </summary>
		/// <param name="data">Data for progress report of IOM</param>
		public void Report(IDEAProgressReport data)
		{
			callback(data);
		}
	}
}
