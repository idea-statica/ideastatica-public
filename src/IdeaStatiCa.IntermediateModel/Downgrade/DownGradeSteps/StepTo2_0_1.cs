using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Downgrade.DownGradeSteps
{
	internal class StepTo201 : BaseStep
	{
		public static Version Version => new(2, 0, 1);


		public override void DoDownStep(SModel _model)
		{
			//rename  LandCode to CountryCode
			_model.ChangeElementName("LandCode", "CountryCode");
		}

		public override Version GetVersion()
		{
			return StepTo201.Version;
		}
	}
}
