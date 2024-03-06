namespace IdeaStatiCa.IOM.VersioningService.Upgrade
{
	public interface IUpgradeService : IVersioningService
	{

		/// <summary>
		/// Upgrade Open Model to latest version
		/// </summary>
		void Upgrade();
	}
}
