using System;

namespace BimApiExample.ViewModels
{
	internal record MessageViewModel
	{
		public MessageViewModel(MessageSeverity severity, string message, Exception? exception = null)
		{
			Message = message;
			Exception = exception;
			Severity = severity;
		}

		public string Message { get; init; }

		public Exception? Exception { get; init; }

		public MessageSeverity Severity { get; init; }

		public override string ToString()
		{
			if (Exception is not null)
			{
				return $"{Message}\r\n{Exception}";
			}

			return Message;
		}
	}
}