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
	/// 3.3.5 - Added ConnectedMember.IsUserEdited (US 33733) - the member was added or kept by the
	/// user in Checkbot instead of coming from the CAD/BIM import.
	/// Upgrade is a no-op, a missing element deserializes to false which is the "CAD-imported" default.
	/// Downgrade strips the property so consumers on &lt; 3.3.5 don't see an unknown element.
	/// </summary>
	internal class Step335 : BaseStep
	{
		public Step335(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.3.5");

		public override Version GetVersion() => Step335.Version;

		public override void DoUpStep(SModel _model)
		{
			_logger.LogInformation($"UpStep {Version}: nothing to upgrade, a missing IsUserEdited means false");
		}

		public override void DoDownStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			foreach (var connectedMember in ConnectionRefTool.FindAll(openModel, "ConnectedMember").ToList())
			{
				connectedMember.RemoveElementProperty("IsUserEdited");
			}
		}
	}
}
