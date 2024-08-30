using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	internal class Step201 : BaseStep
	{
		/// <summary>
		/// rename connection point
		/// </summary>
		/// <param name="logger"></param>
		public Step201(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("2.0.1");

		public override Version GetVersion()
		{
			return Step201.Version;
		}

		public override void DoDownStep(SModel _model)
		{
			var openModel = _model.GetModelElement();

			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}


			var connectionDataList = openModel.GetElements("Connections;ConnectionData");

			if (connectionDataList != null && connectionDataList.Any())
			{
				foreach (ISIntermediate connectionData in connectionDataList)
				{
					_logger.LogInformation("Change Element Property Name  ConenctionPointId => ConnectionPointId");
					connectionData.ChangeElementPropertyName("ConnectionPointId", "ConenctionPointId");
				}
			}
		}

		public override void DoUpStep(SModel _model)
		{
			var openModel = _model.GetModelElement();

			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}


			var connectionDataList = openModel.GetElements("Connections;ConnectionData");

			if (connectionDataList != null && connectionDataList.Any())
			{
				foreach (ISIntermediate connectionData in connectionDataList)
				{
					_logger.LogInformation("Change Element Property Name ConnectionPointId => ConenctionPointId ");
					connectionData.ChangeElementPropertyName("ConenctionPointId", "ConnectionPointId");
				}
			}
		}
	}
}
