using IdeaStatiCa.IntermediateModel.Upgrade.UpgradeSteps;

namespace IdeaStatiCa.IntermediateModel.Upgrade
{
	public partial class IRUpgradeService
	{
		private SortedDictionary<Version, IIRUpgradeStep> _upgradeSteps = new SortedDictionary<Version, IIRUpgradeStep>();
		private void RegisterUpgradeSteps()
		{

			RedgisterStep(new StepTo202());
			RedgisterStep(new StepTo203());
			RedgisterStep(new StepTo201());
		}

		private void RedgisterStep(IIRUpgradeStep step)
		{
			_upgradeSteps.Add(step.GetVersion(), step);
		}
	}

}
