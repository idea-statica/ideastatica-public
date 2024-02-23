using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Upgrade.UpgradeSteps
{
	internal class StepTo201 : BaseStep
	{
		public static Version Version => new(2, 0, 1);


		public override void DoUpStep(SModel _model)
		{
			base.DoUpStep(_model);
			//rename  CountryCode to LandCode
			_model.ChangeElementName("CountryCode", "LandCode");
		}

		public override Version GetVersion()
		{
			return StepTo201.Version;
		}
	}
}
