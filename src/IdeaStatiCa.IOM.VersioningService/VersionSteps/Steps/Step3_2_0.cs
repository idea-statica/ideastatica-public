using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Globalization;
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

			DowngradeCutBeamByBeamWelds(openModel);
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

			UpgradeCutBeamByBeamWelds(openModel);
			UpgradeAnchorGrids(openModel);
		}

		private void DowngradeCutBeamByBeamWelds(ISIntermediate openModel)
		{
			var cutBeamByBeams = openModel.GetElements("Connections;ConnectionData;CutBeamByBeams;CutBeamByBeamData")?.ToList();
			if (cutBeamByBeams == null)
			{
				return;
			}

			foreach (SObject cutBeam in cutBeamByBeams.OfType<SObject>())
			{
				var webWeld = cutBeam.GetElements("WebWeld")?.FirstOrDefault() as SObject;
				var flangeWeld = cutBeam.GetElements("FlangeWeld")?.FirstOrDefault() as SObject;

				if (webWeld != null || flangeWeld != null)
				{
					var webThicknessStr = webWeld?.GetElementValue("Thickness") ?? "0";
					var flangeThicknessStr = flangeWeld?.GetElementValue("Thickness") ?? "0";

					double.TryParse(webThicknessStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var webThickness);
					double.TryParse(flangeThicknessStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var flangeThickness);

					var maxThickness = Math.Max(webThickness, flangeThickness);
					var weldType = webWeld?.GetElementValue("WeldType") ?? flangeWeld?.GetElementValue("WeldType") ?? "NotSpecified";

					cutBeam.CreateElementProperty("WeldThickness", maxThickness.ToString(CultureInfo.InvariantCulture));
					cutBeam.CreateElementProperty("WeldType", weldType);
				}

				cutBeam.RemoveElementProperty("WebWeld");
				cutBeam.RemoveElementProperty("FlangeWeld");
			}
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

		private void UpgradeCutBeamByBeamWelds(ISIntermediate openModel)
		{
			var cutBeamByBeams = openModel.GetElements("Connections;ConnectionData;CutBeamByBeams;CutBeamByBeamData")?.ToList();
			if (cutBeamByBeams == null)
			{
				return;
			}

			foreach (SObject cutBeam in cutBeamByBeams.OfType<SObject>())
			{
				// Skip if already has WebWeld (already upgraded)
				var existingWebWeld = cutBeam.GetElements("WebWeld")?.FirstOrDefault();
				if (existingWebWeld != null)
				{
					continue;
				}

				var thickness = cutBeam.GetElementValue("WeldThickness") ?? "0";
				var weldType = cutBeam.GetElementValue("WeldType") ?? "NotSpecified";

				SObject webWeldObj = cutBeam.CreateElementProperty("WebWeld");
				webWeldObj.CreateElementProperty("Thickness", thickness);
				webWeldObj.CreateElementProperty("WeldType", weldType);

				SObject flangeWeldObj = cutBeam.CreateElementProperty("FlangeWeld");
				flangeWeldObj.CreateElementProperty("Thickness", thickness);
				flangeWeldObj.CreateElementProperty("WeldType", weldType);

				cutBeam.RemoveElementProperty("WeldThickness");
				cutBeam.RemoveElementProperty("WeldType");
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
