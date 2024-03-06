using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps
{
	public interface IDowngradeStep : IStep
	{
		/// <summary>
		/// Do downgrade step witch apply changes on model
		/// </summary>
		/// <param name="_model"></param>
		void DoDownStep(SModel _model);

		/// <summary>
		/// Downgrade Version - change version by downgrading
		/// </summary>
		/// <param name="_model"></param>
		void DowngradeVersion(SModel _model);
	}
}
