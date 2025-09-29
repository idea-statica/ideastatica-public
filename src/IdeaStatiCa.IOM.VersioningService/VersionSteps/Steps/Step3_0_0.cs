using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

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
			//ISIntermediate openModel = _model.GetModelElement();

			//if (openModel == null)
			//{
			//	_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
			//	return;
			//}

			//IEnumerable<ISIntermediate> elements = openModel.GetElements("Element1D;Element1D");

			//if (elements == null)
			//{
			//	return;
			//}

			//foreach (ISIntermediate element in elements)
			//{
			//	element.RemoveElementProperty("EccentricityReference");
			//	element.RemoveElementProperty("InsertionPoint");

			//	ISIntermediate eccBegin = element.TakeElementProperty("EccentricityBegin");
			//	double eccBeginX = double.Parse(eccBegin.GetElementValue("X"));
			//	double eccBeginY = double.Parse(eccBegin.GetElementValue("Y"));
			//	double eccBeginZ = double.Parse(eccBegin.GetElementValue("Z"));

			//	ISIntermediate eccEnd = element.TakeElementProperty("EccentricityEnd");
			//	double eccEndX = double.Parse(eccEnd.GetElementValue("X"));
			//	double eccEndY = double.Parse(eccEnd.GetElementValue("Y"));
			//	double eccEndZ = double.Parse(eccEnd.GetElementValue("Z"));

			//	// TODO conversion from GCS to LCS

			//	element.CreateElementProperty("EccentricityBeginX", eccBeginX.ToString());
			//	element.CreateElementProperty("EccentricityBeginY", eccBeginY.ToString());
			//	element.CreateElementProperty("EccentricityBeginZ", eccBeginZ.ToString());
			//	element.CreateElementProperty("EccentricityEndX", eccEndX.ToString());
			//	element.CreateElementProperty("EccentricityEndY", eccEndY.ToString());
			//	element.CreateElementProperty("EccentricityEndZ", eccEndZ.ToString());
			//}
		}

		public override void DoUpStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();

			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			List<ISIntermediate> members = openModel.GetElements("Member1D;Member1D")?.ToList();
			if (members is null)
			{
				return;
			}

			foreach (SObject member in members.OfType<SObject>())
			{
				List<SObject> elements = member.Properties["Elements1D"]
					.GetElements("ReferenceElement")
					.Select(x => x.ResolveReferenceElement(_model))
					.ToList();

				if (elements.Count == 0)
				{
					continue;
				}

				var eccBegin = GetEccentricity(elements[0], false);
				var eccEnd = GetEccentricity(elements[elements.Count - 1], true);

				foreach (ISIntermediate element in elements)
				{
					element.RemoveElementProperty("EccentricityBeginX");
					element.RemoveElementProperty("EccentricityBeginY");
					element.RemoveElementProperty("EccentricityBeginZ");
					element.RemoveElementProperty("EccentricityEndX");
					element.RemoveElementProperty("EccentricityEndY");
					element.RemoveElementProperty("EccentricityEndZ");
				}

				SObject eccBeginObj = member.CreateElementProperty("EccentricityBegin");
				eccBeginObj.CreateElementProperty("X", eccBegin.X.ToString());
				eccBeginObj.CreateElementProperty("Y", eccBegin.Y.ToString());
				eccBeginObj.CreateElementProperty("Z", eccBegin.Z.ToString());

				SObject eccEndObj = member.CreateElementProperty("EccentricityEnd");
				eccEndObj.CreateElementProperty("X", eccEnd.X.ToString());
				eccEndObj.CreateElementProperty("Y", eccEnd.Y.ToString());
				eccEndObj.CreateElementProperty("Z", eccEnd.Z.ToString());

				member.CreateElementProperty("InsertionPoint", "CenterOfGravity");
				member.CreateElementProperty("EccentricityReference", "LocalCoordinateSystem");
			}
		}

		private static (double X, double Y, double Z) GetEccentricity(SObject element, bool end)
		{
			string prefix = end ? "EccentricityEnd" : "EccentricityBegin";

			double x = double.Parse(element.GetElementValue(prefix + "X"));
			double y = double.Parse(element.GetElementValue(prefix + "Y"));
			double z = double.Parse(element.GetElementValue(prefix + "Z"));

			return (x, y, z);
		}
	}
}
