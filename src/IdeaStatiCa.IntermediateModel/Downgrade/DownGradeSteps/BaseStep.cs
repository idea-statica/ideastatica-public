using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Downgrade.DownGradeSteps
{
	internal class BaseStep : IIRDowngradeStep
	{
		public virtual void DoDownStep(SModel _model)
		{
		}

		public void DowngradeVersion(SModel _model)
		{
			_model.ChangeElementValue("Version", GetVersion().ToString());
		}

		public virtual Version GetVersion()
		{
			return new(0, 0, 0);
		}
	}
}
