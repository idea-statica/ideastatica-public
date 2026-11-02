using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.IOM.VersioningService.Tools;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	/// <summary>
	/// 3.3.1 - PlateData.Material: name string &lt;-&gt; ReferenceElement (MatSteel).
	/// </summary>
	internal class Step331 : BaseStep
	{
		public Step331(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.3.1");

		public override Version GetVersion() => Step331.Version;

		public override void DoUpStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			var nameToId = ConnectionRefTool.BuildNameToId(openModel, "MatSteel");
			foreach (var plate in ConnectionRefTool.FindAll(openModel, "PlateData").ToList())
			{
				ConnectionRefTool.StringToReference(plate, "Material", "MatSteel", nameToId, _logger);
			}
		}

		public override void DoDownStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			var idToName = ConnectionRefTool.BuildIdToName(openModel, "MatSteel");
			foreach (var plate in ConnectionRefTool.FindAll(openModel, "PlateData").ToList())
			{
				ConnectionRefTool.ReferenceToString(plate, "Material", idToName, _logger);
			}
		}
	}
}
