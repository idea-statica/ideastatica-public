using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.OpenModel.VersioningService.VersionSteps.Steps
{
	internal class Step200 : BaseStep
	{
		/// <summary>
		/// Initial step for make version from 2 to 2.0.0
		/// </summary>
		/// <param name="logger"></param>
		public Step200(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => new(2, 0, 0);

		public override Version GetVersion()
		{
			return Step200.Version;
		}
	}
}
