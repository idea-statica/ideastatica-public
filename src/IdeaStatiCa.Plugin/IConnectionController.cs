using System;

namespace IdeaStatiCa.Plugin
{
	public interface IConnectionController
	{
		event EventHandler ConnectionAppExited;

		bool IsConnected { get; }

		int OpenProject(string fileName);

		int CloseProject();

	}
}
