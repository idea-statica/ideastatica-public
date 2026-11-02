using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.IOM.VersioningService.Tools;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	/// <summary>
	/// 3.3.5 - WeldData, CutDataBase (CutData, CutBeamByBeamData) and FastenerGridBase
	/// (BoltGrid, AnchorGrid, PinGrid) gain OriginalModelId - the identification of the object
	/// in the model of the source application.
	/// </summary>
	internal class Step335 : BaseStep
	{
		private static readonly string[] TypeNames = new[]
		{
			"WeldData",
			"CutData",
			"CutBeamByBeamData",
			"BoltGrid",
			"AnchorGrid",
			"PinGrid",
		};

		public Step335(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.3.5");

		public override Version GetVersion() => Step335.Version;

		public override void DoUpStep(SModel _model)
		{
			// Nothing to materialize - an empty OriginalModelId is a legitimate value, a source
			// application does not have to provide an identity. The version itself is stamped by
			// BaseStep.UpgradeVersion.
			_logger.LogInformation($"UpStep {Version}: no data change, OriginalModelId stays undefined for older payloads");
		}

		public override void DoDownStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			foreach (string typeName in TypeNames)
			{
				foreach (var item in ConnectionRefTool.FindAll(openModel, typeName).ToList())
				{
					item.RemoveElementProperty("OriginalModelId");
				}
			}
		}
	}
}
