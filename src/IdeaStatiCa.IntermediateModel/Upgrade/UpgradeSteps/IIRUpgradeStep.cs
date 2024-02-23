using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Upgrade.UpgradeSteps
{
	internal interface IIRUpgradeStep
	{
		Version GetVersion();

		void DoUpStep(SModel _model);
	}
}
