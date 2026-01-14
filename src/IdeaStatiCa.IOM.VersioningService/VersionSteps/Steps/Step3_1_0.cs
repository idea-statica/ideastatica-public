using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	internal class Step310 : BaseStep
	{
		public Step310(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.1.0");

		public override Version GetVersion() => Step310.Version;

		public override void DoDownStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			List<ISIntermediate> csss = openModel.GetElements("CrossSection;CrossSection")?.ToList();
			if (csss != null)
			{
				foreach (SObject css in csss.OfType<SObject>())
				{
					string cssType = css.GetElementValue("xsi:type");
					if (cssType == null || cssType != "GeneralCrossSection")
					{
						continue;
					}

					ProcessCrossSection(css);
				}
			}
		}

		public override void DoUpStep(SModel _model)
		{
			// No action needed in UpStep
		}

		private static void ProcessCrossSection(SObject css)
		{
			SObject cssNew = new SObject();

			cssNew.AddElementProperty(new SAttribute()
			{
				Prefix = "xsi",
				LocalName = "type",
				Value = "GeneralCrossSection",
			});
			cssNew.CreateElementProperty("Id", css.GetElementValue("Id"));
			cssNew.CreateElementProperty("Name", css.GetElementValue("Name"));
			cssNew.CreateElementProperty("IsInPrincipal", css.GetElementValue("IsInPrincipal"));
			cssNew.CreateElementProperty("CrossSectionType", "UniqueName");

			SList parameters = cssNew.CreateListProperty("Parameters");
			SObject paramUniqueName = new SObject()
			{
				TypeName = "Parameter"
			};
			parameters.AddElementProperty(paramUniqueName);

			paramUniqueName.AddElementProperty(new SAttribute()
			{
				Prefix = "xsi",
				LocalName = "type",
				Value = "ParameterString",
			});
			paramUniqueName.CreateElementProperty("Name", "UniqueName");
			paramUniqueName.CreateElementProperty("Value", css.GetElementValue("Name"));

			css.Properties = cssNew.Properties;
		}
	}
}
