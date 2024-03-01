namespace IdeaStatiCa.OpenModel.VersioningService.Downgrade
{
	public interface IDowngradeService : IVersioningService
	{

		/// <summary>
		/// Downgrade to specific version
		/// </summary>
		public void Downgrade(Version version);

		public void Downgrade(string version);

		public IEnumerable<Version> GetVersionsToDowngrade();
	}
}
