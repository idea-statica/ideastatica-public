using IdeaStatiCa.OpenModel.VersioningService.VersionSteps;

namespace IdeaStatiCa.OpenModel.VersioningService.Configuration
{
	public interface IConfigurationStepService
	{
		/// <summary>
		/// Get sequence of downgrade steps
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDowngradeStep> DowngradeSteps();

		/// <summary>
		/// Get sequence of upgrade steps
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IUpgradeStep> UpgradeSteps();


		/// <summary>
		/// Get version of latest step
		/// </summary>
		/// <returns></returns>
		public Version GetLatestStepVersion();

	}
}
