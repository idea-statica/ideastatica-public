using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	internal class Step220 : BaseStep
	{
		public Step220(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("2.2.0");

		public override Version GetVersion()
		{
			return Step220.Version;
		}

		public override void DoDownStep(SModel _model)
		{
			var openModel = _model.GetModelElement();

			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			var loadsOnSurface = openModel.GetElements("LoadOnSurface");

			if (loadsOnSurface != null) 
			{
				foreach (var load in loadsOnSurface.ToList())
				{
					load.RemoveElementProperty("ReferencedGeometry");
				}
			}
		}

		public override void DoUpStep(SModel _model)
		{			
			var openModel = _model.GetModelElement();

			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			var loadsOnSurface = openModel.GetElements("LoadOnSurface");

			if (loadsOnSurface != null)
			{
				foreach (var load in loadsOnSurface.ToList())
				{
					load.CreateElementProperty("ReferencedGeometry", "");
				}
			}
		}
	}
}
