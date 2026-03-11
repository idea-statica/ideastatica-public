using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
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

			// Strip WebWeld and FlangeWeld elements from CutBeamByBeamData nodes
			var cutBeamByBeams = openModel.GetElements("Connections;ConnectionData;CutBeamByBeams;CutBeamByBeamData")?.ToList();
			if (cutBeamByBeams == null)
			{
				return;
			}

			foreach (SObject cutBeam in cutBeamByBeams.OfType<SObject>())
			{
				// Extract values from WebWeld to re-populate legacy WeldThickness/WeldType
				var webWeld = cutBeam.GetElements("WebWeld")?.FirstOrDefault() as SObject;
				if (webWeld != null)
				{
					var thickness = webWeld.GetElementValue("Thickness") ?? "0";
					var weldType = webWeld.GetElementValue("WeldType") ?? "NotSpecified";

					cutBeam.CreateElementProperty("WeldThickness", thickness);
					cutBeam.CreateElementProperty("WeldType", weldType);
				}

				cutBeam.RemoveElementProperty("WebWeld");
				cutBeam.RemoveElementProperty("FlangeWeld");
			}
>>>>>>> ad444292 (Weld support in IOM)
		}

		public override void DoUpStep(SModel _model)
		{
			// No action needed - null WebWeld/FlangeWeld properties are handled by import fallback logic
		}
	}
}
