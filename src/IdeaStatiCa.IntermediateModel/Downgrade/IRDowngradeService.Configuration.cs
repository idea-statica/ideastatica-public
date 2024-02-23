using IdeaStatiCa.IntermediateModel.Downgrade.DownGradeSteps;

namespace IdeaStatiCa.IntermediateModel.Downgrade
{
	public partial class IRDowngradeService
	{
		private SortedDictionary<Version, IIRDowngradeStep> _downgradeSteps = new SortedDictionary<Version, IIRDowngradeStep>();
		private void RegisterDowngradeSteps()
		{
			RedgisterStep(new StepTo200());
			RedgisterStep(new StepTo201());
			RedgisterStep(new StepTo202());
		}

		private void RedgisterStep(IIRDowngradeStep step)
		{
			_downgradeSteps.Add(step.GetVersion(), step);
		}
	}
}
