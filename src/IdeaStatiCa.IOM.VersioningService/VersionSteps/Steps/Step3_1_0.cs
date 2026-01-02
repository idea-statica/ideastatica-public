using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
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

			ISIntermediate element = openModel.GetElements("CrossSection").FirstOrDefault();
			if(element != null)
			{
				// take will remove the element from the parent
				SList crossSectionList = element.TryTakeElementProperty("CrossSection") as SList;
				if(crossSectionList != null)
				{
					foreach (var el in crossSectionList.GetElements("CrossSection").ToList())
					{
						string cssType = el.GetElementValue("xsi:type");
						if (cssType != null && cssType == "GeneralCrossSection")
						{
							crossSectionList.RemoveElementProperty(el);

							// TODO: we can add cross-section as CrossSectionParameter with UniqueName (of the original css) only
							// since downgrade is not used anywhere, we leave it like this for now
						}
					}
					// add back the modified list
					element.AddElementProperty(crossSectionList);
				}
			}
		}

		public override void DoUpStep(SModel _model)
		{
			// No action needed in UpStep
		}
	}
}
