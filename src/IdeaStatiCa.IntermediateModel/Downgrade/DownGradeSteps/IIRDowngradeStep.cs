using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Downgrade.DownGradeSteps
{
	internal interface IIRDowngradeStep
	{
		Version GetVersion();

		void DoDownStep(SModel _model);

		void DowngradeVersion(SModel _model);
	}
}
