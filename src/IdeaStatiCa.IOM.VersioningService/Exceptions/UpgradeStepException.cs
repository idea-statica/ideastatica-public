using System;

namespace IdeaStatiCa.IOM.VersioningService.Exceptions
{
	public class UpgradeStepException : Exception
	{
		public UpgradeStepException(Version version)
			: base($"Upgrade process fail during step to {version}")
		{
		}

		public UpgradeStepException(Version version, Exception inner)
			: base($"Upgrade process fail during step to {version}", inner)
		{
		}

		public UpgradeStepException(Version version, string message)
			: base($"Upgrade process fail during step to {version} {message}")
		{
		}

		public UpgradeStepException(Version version, string message, Exception inner)
			: base($"Upgrade process fail during step to {version} {message}", inner)
		{
		}
	}
}
