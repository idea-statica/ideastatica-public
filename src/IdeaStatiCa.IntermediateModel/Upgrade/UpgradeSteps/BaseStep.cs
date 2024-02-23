using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Upgrade.UpgradeSteps
{
	internal abstract class BaseStep : IIRUpgradeStep
	{
		public virtual void DoUpStep(SModel _model)
		{
			_model.ChangeElementValue("Version", GetVersion().ToString());
		}

		public virtual Version GetVersion()
		{
			return new(0, 0, 0);
		}
	}
}
