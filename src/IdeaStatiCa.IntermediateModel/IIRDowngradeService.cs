using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel
{
	public interface IIRDowngradeService
	{
		public void LoadModel(SModel model);

		/// <summary>
		/// Downgrade to specific version
		/// </summary>
		public void Downgrade(Version version);

		public void Downgrade(string version);

		public bool IsModelActual();

		public IEnumerable<Version> GetVersionsToDowngrade();
	}
}
