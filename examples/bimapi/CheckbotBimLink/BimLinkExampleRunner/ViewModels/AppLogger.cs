using CommunityToolkit.Mvvm.Input;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Public;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace BimLinkExampleRunner.ViewModels
{
	/// <summary>
	/// The log of the link, shown in the window. The link logs from any thread, so each message is added on the UI
	/// thread.
	/// </summary>
	internal class AppLogger : IPluginLogger
	{
		private readonly Dispatcher uiDispatcher;

		public AppLogger(Dispatcher uiDispatcher)
		{
			this.uiDispatcher = uiDispatcher;
			ClearCommand = new RelayCommand(Messages.Clear);
		}

		/// <summary>
		/// Removes all messages.
		/// </summary>
		public ICommand ClearCommand { get; }

		/// <summary>
		/// The messages, oldest first.
		/// </summary>
		public ObservableCollection<MessageViewModel> Messages { get; } = new ObservableCollection<MessageViewModel>();

		public void LogError(string message, Exception ex = null)
		{
			Log(MessageSeverity.Error, message, ex);
		}

		public void LogWarning(string message, Exception ex = null)
		{
			Log(MessageSeverity.Warning, message, ex);
		}

		public void LogInformation(string message, Exception ex = null)
		{
			Log(MessageSeverity.Information, message, ex);
		}

		public void LogDebug(string message, Exception ex = null)
		{
			Log(MessageSeverity.Debug, message, ex);
		}

		/// <summary>
		/// Not shown: the import writes a trace message for every object it imports.
		/// </summary>
		public void LogTrace(string message, Exception ex = null)
		{
		}

		/// <summary>
		/// Not shown: usage events are statistics for IDEA StatiCa, not messages for the user.
		/// </summary>
		public void LogEventInformation(IIdeaUserEvent userEvent, string screenName = null, Dictionary<int, string> eventCustomDimensions = null)
		{
		}

		private void Log(MessageSeverity severity, string message, Exception ex)
		{
			uiDispatcher.BeginInvoke(new Action(() => Messages.Add(new MessageViewModel(severity, message, ex))));
		}
	}
}
