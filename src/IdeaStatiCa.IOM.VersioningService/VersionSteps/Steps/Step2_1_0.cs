
using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	internal class Step210 : BaseStep
	{
		/// <summary>
		/// Add pins to model
		/// </summary>
		/// <param name="logger"></param>
		public Step210(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("2.1.0");

		public override Version GetVersion()
		{
			return Step210.Version;
		}

		public override void DoDownStep(SModel _model)
		{
			var openModel = _model.GetModelElement();

			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			//remove definition of pin
			_logger.LogInformation($"OpenModel remove pin collection");
			var pinLists = openModel.GetElements("Pin");

			if (pinLists != null)
			{
				foreach (var pin in pinLists.ToList())
				{
					openModel.RemoveElementProperty(pin);
				}
			}

			//remove pingrids
			_logger.LogInformation($"OpenModel remove connection PinGrids");
			var connections = openModel.GetElements("Connections:ConnectionData");
			if (connections != null && connections.Any())
			{
				foreach (var connection in connections)
				{
					var pinGrids = connection.GetElements("PinGrids");

					if (pinGrids != null && pinGrids.Any())
					{
						foreach (var pinGrid in pinGrids)
						{
							connection.RemoveElementProperty(pinGrid);
						}
					}
				}
			}
		}

		public override void DoUpStep(SModel _model)
		{
		}
	}
}
