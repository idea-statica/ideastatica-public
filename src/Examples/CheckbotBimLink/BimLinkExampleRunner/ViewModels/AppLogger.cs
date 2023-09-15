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
	internal class AppLogger : IPluginLogger
	{
		private readonly Dispatcher uiDispatcher;

		public AppLogger(Dispatcher uiDispatcher)
		{
			this.uiDispatcher = uiDispatcher;

			ClearCommand = new RelayCommand(OnClear);
		}

		public ICommand ClearCommand { get; }

		public bool EnableTrace { get; set; }

		public bool EnableDebug { get; set; }

		public ObservableCollection<MessageViewModel> Messages { get; } = new ObservableCollection<MessageViewModel>();

		public void LogDebug(string message, Exception ex = null)
		{
			if (EnableDebug)
			{
				Log(MessageSeverity.Debug, message, ex);
			}
		}

		public void LogError(string message, Exception ex = null)
		{
			Log(MessageSeverity.Error, message, ex);
		}

		public void LogEventInformation(IIdeaUserEvent userEvent, string screenName = null, Dictionary<int, string> eventCustomDimensions = null)
		{
		}

		public void LogInformation(string message, Exception ex = null)
		{
			Log(MessageSeverity.Information, message, ex);
		}

		public void LogTrace(string message, Exception ex = null)
		{
			if (EnableTrace)
			{
				Log(MessageSeverity.Trace, message, ex);
			}
		}

		public void LogWarning(string message, Exception ex = null)
		{
			Log(MessageSeverity.Warning, message, ex);
		}

		private void Log(MessageSeverity severity, string message, Exception ex)
		{
			uiDispatcher.BeginInvoke(new Action(() =>
			{
				Messages.Add(new MessageViewModel(severity, message, ex));
			}));
		}

		private void OnClear()
		{
			Messages.Clear();
		}
	}
}