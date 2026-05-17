using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	/// <summary>
	/// US 33733 — IOM 3.3.0 added IdeaRS.OpenModel.Connection.ConnectedMember.IsUserEdited.
	/// Upgrade is a no-op (DataContractSerializer treats missing element as default false).
	/// Downgrade strips the property so consumers on &lt; 3.3.0 don't see an unknown element.
	/// </summary>
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

			DowngradeConnectedMembers(openModel);
		}

		public override void DoUpStep(SModel _model)
		{
			// No-op: DataContractSerializer assigns default(false) when IsUserEdited is missing
			// from a pre-3.3.0 XML, which matches "CAD-imported, never user-edited" semantics.
		}

		private void DowngradeConnectedMembers(ISIntermediate openModel)
		{
			var connectedMembers = openModel
				.GetElements("Connections;ConnectionData;ConnectedBeams;ConnectedMember")
				?.ToList();
			if (connectedMembers == null)
			{
				return;
			}

			foreach (SObject cm in connectedMembers.OfType<SObject>())
			{
				cm.RemoveElementProperty("IsUserEdited");
			}
		}
	}
}
