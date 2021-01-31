using System;

namespace IdeaStatiCa.Plugin
{
	public delegate void ISEventHandler(object sender, ISEventArgs e);

	public enum AppStatus
	{
		Started,
		Finished
	}

	public class ISEventArgs : EventArgs
	{
		public AppStatus Status { get; set; }
	}
}