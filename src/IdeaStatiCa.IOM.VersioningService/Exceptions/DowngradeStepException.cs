using System;

namespace IdeaStatiCa.IOM.VersioningService.Exceptions
{
	public class DowngradeStepException : Exception
	{
		public DowngradeStepException(Version version)
			: base($"Downgrade process fail during step to {version}")
		{
		}

		public DowngradeStepException(Version version, Exception inner)
			: base($"Downgrade process fail during step to {version}", inner)
		{
		}

		public DowngradeStepException(Version version, string message)
			: base($"Downgrade process fail during step to {version} {message}")
		{
		}

		public DowngradeStepException(Version version, string message, Exception inner)
			: base($"Downgrade process fail during step to {version} {message}", inner)
		{
		}
	}
}
