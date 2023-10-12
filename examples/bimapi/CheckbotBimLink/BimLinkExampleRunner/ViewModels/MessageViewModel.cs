using System;

namespace BimLinkExampleRunner.ViewModels
{
	internal class MessageViewModel
	{
		public MessageViewModel(MessageSeverity severity, string message, Exception exception = null)
		{
			Message = message;
			Exception = exception;
			Severity = severity;
		}

		public string Message { get; set; }

		public Exception Exception { get; set; }

		public MessageSeverity Severity { get; set; }

		public override string ToString()
		{
			if (Exception != null)
			{
				return $"{Message}\r\n{Exception}";
			}

			return Message;
		}
	}
}