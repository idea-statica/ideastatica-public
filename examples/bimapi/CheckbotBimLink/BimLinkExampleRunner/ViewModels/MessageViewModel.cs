using System;

namespace BimLinkExampleRunner.ViewModels
{
	/// <summary>
	/// One message of the log.
	/// </summary>
	internal class MessageViewModel
	{
		public MessageViewModel(MessageSeverity severity, string message, Exception exception)
		{
			Severity = severity;
			Message = message;
			Exception = exception;
		}

		public MessageSeverity Severity { get; }

		public string Message { get; }

		/// <summary>
		/// The exception logged with the message, or null.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// The message with its exception, as Ctrl+C copies it.
		/// </summary>
		public override string ToString()
		{
			return Exception == null ? Message : Message + Environment.NewLine + Exception;
		}
	}
}
