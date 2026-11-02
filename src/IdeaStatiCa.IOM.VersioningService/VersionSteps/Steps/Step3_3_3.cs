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
	/// 3.3.3 - CutData and CutBeamByBeamData gain a unique Id (from the new CutDataBase : OpenElementId).
	/// </summary>
	internal class Step333 : BaseStep
	{
		public Step333(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.3.3");

		public override Version GetVersion() => Step333.Version;

		public override void DoUpStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			AddIds(openModel, "CutData");
			AddIds(openModel, "CutBeamByBeamData");
		}

		public override void DoDownStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			foreach (var cut in ConnectionRefTool.FindAll(openModel, "CutData").ToList())
			{
				cut.RemoveElementProperty("Id");
			}
			foreach (var cut in ConnectionRefTool.FindAll(openModel, "CutBeamByBeamData").ToList())
			{
				cut.RemoveElementProperty("Id");
			}
		}

		private static void AddIds(ISIntermediate openModel, string typeName)
		{
			int id = 1;
			foreach (var cut in ConnectionRefTool.FindAll(openModel, typeName).ToList())
			{
				if (!cut.Properties.ContainsKey("Id"))
				{
					cut.CreateElementProperty("Id").ChangeElementValue((id++).ToString());
				}
			}
		}
	}
}
