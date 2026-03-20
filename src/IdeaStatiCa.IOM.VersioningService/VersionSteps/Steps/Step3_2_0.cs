using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	internal class Step320 : BaseStep
	{
		public Step320(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.2.0");

		public override Version GetVersion() => Step320.Version;

		public override void DoDownStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			DowngradeAnchorGrids(openModel);
		}

		public override void DoUpStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			UpgradeAnchorGrids(openModel);
		}

		private void DowngradeAnchorGrids(ISIntermediate openModel)
		{
			var anchorGrids = openModel.GetElements("Connections;ConnectionData;AnchorGrids;AnchorGrid")?.ToList();
			if (anchorGrids == null)
			{
				return;
			}

			foreach (SObject anchorGrid in anchorGrids.OfType<SObject>())
			{
				anchorGrid.RemoveElementProperty("AnchorInstallationProcess");
				anchorGrid.RemoveElementProperty("HeadedStudHeadDiameter");
				anchorGrid.RemoveElementProperty("ReinforcementMandrelDiameter");
			}
		}

		private void UpgradeAnchorGrids(ISIntermediate openModel)
		{
			var anchorGrids = openModel.GetElements("Connections;ConnectionData;AnchorGrids;AnchorGrid")?.ToList();
			if (anchorGrids == null)
			{
				return;
			}

			foreach (SObject anchorGrid in anchorGrids.OfType<SObject>())
			{
				if (anchorGrid.GetElements("AnchorInstallationProcess")?.FirstOrDefault() != null)
				{
					continue;
				}

				anchorGrid.CreateElementProperty("AnchorInstallationProcess", "PostInstalled");
				anchorGrid.CreateElementProperty("HeadedStudHeadDiameter", "0");
				anchorGrid.CreateElementProperty("ReinforcementMandrelDiameter", "0");
			}
		}
	}
}
