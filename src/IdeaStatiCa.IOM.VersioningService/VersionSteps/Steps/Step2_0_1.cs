using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.IOM.VersioningService.Tools;
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

					var cpIds = connectionData.GetElements("ConnectionPoint;Id");
					if (cpIds != null && cpIds.Any())
					{
						var conId = cpIds.First().GetElementValue(null);
						connectionData.CreateElementProperty("ConenctionPointId").ChangeElementValue(conId);

						connectionData.RemoveElementProperty("ConnectionPoint");
					}
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
					var conId = connectionData.TryGetElementValue("ConenctionPointId");

					connectionData.RemoveElementProperty("ConenctionPointId");

					var cpIRobj = connectionData.CreateElementProperty("ConnectionPoint");

					//for cases where is tag connection point defined
					if (string.IsNullOrWhiteSpace(conId))
					{
						_logger.LogDebug($"Connection data not contains connection point id => set default 0");
						conId = "0";
					}

					IRIOMTool.CreateIOMReferenceElement(cpIRobj, "ConnectionPoint", conId);
				}
			}
		}
	}
}
