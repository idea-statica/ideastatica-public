using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	internal class Step300 : BaseStep
	{
		public Step300(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.0.0");

		public override Version GetVersion() => Step300.Version;

		public override void DoDownStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();

			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			IEnumerable<ISIntermediate> elements = openModel.GetElements("Element1D;Element1D");

			if (elements == null)
			{
				return;
			}

			foreach (ISIntermediate element in elements)
			{
				element.RemoveElementProperty("EccentricityReference");
				element.RemoveElementProperty("CardinalPoint");

				ISIntermediate eccBegin = element.TakeElementProperty("EccentricityBegin");
				double eccBeginX = double.Parse(eccBegin.GetElementValue("X"));
				double eccBeginY = double.Parse(eccBegin.GetElementValue("Y"));
				double eccBeginZ = double.Parse(eccBegin.GetElementValue("Z"));

				ISIntermediate eccEnd = element.TakeElementProperty("EccentricityEnd");
				double eccEndX = double.Parse(eccEnd.GetElementValue("X"));
				double eccEndY = double.Parse(eccEnd.GetElementValue("Y"));
				double eccEndZ = double.Parse(eccEnd.GetElementValue("Z"));

				// TODO conversion from GCS to LCS

				element.CreateElementProperty("EccentricityBeginX", eccBeginX.ToString());
				element.CreateElementProperty("EccentricityBeginY", eccBeginY.ToString());
				element.CreateElementProperty("EccentricityBeginZ", eccBeginZ.ToString());
				element.CreateElementProperty("EccentricityEndX", eccEndX.ToString());
				element.CreateElementProperty("EccentricityEndY", eccEndY.ToString());
				element.CreateElementProperty("EccentricityEndZ", eccEndZ.ToString());
			}
		}

		public override void DoUpStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();

			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			IEnumerable<ISIntermediate> elements = openModel.GetElements("Element1D;Element1D");

			if (elements == null)
			{
				return;
			}

			foreach (ISIntermediate element in elements)
			{
				string eccBeginX = element.TakeElementProperty("EccentricityBeginX").GetElementValue(null);
				string eccBeginY = element.TakeElementProperty("EccentricityBeginY").GetElementValue(null);
				string eccBeginZ = element.TakeElementProperty("EccentricityBeginZ").GetElementValue(null);
				string eccEndX = element.TakeElementProperty("EccentricityEndX").GetElementValue(null);
				string eccEndY = element.TakeElementProperty("EccentricityEndY").GetElementValue(null);
				string eccEndZ = element.TakeElementProperty("EccentricityEndZ").GetElementValue(null);

				SObject eccBegin = element.CreateElementProperty("EccentricityBegin");
				eccBegin.CreateElementProperty("X", eccBeginX);
				eccBegin.CreateElementProperty("Y", eccBeginY);
				eccBegin.CreateElementProperty("Z", eccBeginZ);

				SObject eccEnd = element.CreateElementProperty("EccentricityEnd");
				eccEnd.CreateElementProperty("X", eccEndX);
				eccEnd.CreateElementProperty("Y", eccEndY);
				eccEnd.CreateElementProperty("Z", eccEndZ);

				element.CreateElementProperty("CardinalPoint", "CenterOfGravity");
				element.CreateElementProperty("EccentricityReference", "LocalCoordinateSystem");
			}
		}
	}
}
