namespace IdeaStatiCa.IntermediateModel.Downgrade.DownGradeSteps
{
	internal class StepTo200 : BaseStep
	{
		public static Version Version => new(2, 0, 0);

		public override Version GetVersion()
		{
			return StepTo200.Version;
		}
	}
}
