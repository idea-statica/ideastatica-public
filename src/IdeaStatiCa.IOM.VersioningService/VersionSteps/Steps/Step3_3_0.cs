using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	internal class Step330 : BaseStep
	{
		public Step330(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.3.0");

		public override Version GetVersion() => Step330.Version;

		public override void DoDownStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			var boltGrids = openModel.GetElements("Connections;ConnectionData;BoltGrids;BoltGrid")?.ToList();
			if (boltGrids == null)
			{
				return;
			}

			foreach (SObject boltGrid in boltGrids.OfType<SObject>())
			{
				boltGrid.RemoveElementProperty("SlottedHoles");
			}
		}

		public override void DoUpStep(SModel _model)
		{
			// SlottedHoles is optional - a missing property means round holes, so there is nothing to materialize on upgrade
			_logger.LogInformation($"UpStep {Version}: nothing to upgrade");
		}
	}
}
