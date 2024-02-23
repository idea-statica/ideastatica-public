namespace IdeaStatiCa.IntermediateModel.Exceptions
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
