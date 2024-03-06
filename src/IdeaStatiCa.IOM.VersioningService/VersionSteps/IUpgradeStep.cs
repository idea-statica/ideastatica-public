using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps
{
	public interface IUpgradeStep : IStep
	{
		/// <summary>
		/// Do upgrade step witch apply changes on model
		/// </summary>
		/// <param name="_model"></param>
		void DoUpStep(SModel _model);

		/// <summary>
		/// Upgrade Version - change version by upgrading
		/// </summary>
		/// <param name="_model"></param>
		void UpgradeVersion(SModel _model);
	}
}
