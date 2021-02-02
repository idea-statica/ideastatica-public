using System;
using System.Windows.Input;

namespace IdeaStatiCa.ConnectionClient.ConHiddenCalcCommands
{
	public interface IUpdateCommand : ICommand
	{
		event EventHandler UpdateFinished;
	}
}
