using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel
{
	public interface IIRUpgradeService
	{
		public void LoadModel(SModel model);

		/// <summary>
		/// Upgrade to latest version
		/// </summary>
		public void Upgrade();

		public bool IsModelActual();

	}
}
