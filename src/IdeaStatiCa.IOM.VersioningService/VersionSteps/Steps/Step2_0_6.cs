using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.IOM.VersioningService.Tools;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	internal class Step206 : BaseStep
	{
		/// <summary>
		/// Change grids connected items
		/// </summary>
		/// <param name="logger"></param>
		public Step206(IPluginLogger logger) : base(logger)
		{

		}

		public static Version Version => Version.Parse("2.0.6");

		public override Version GetVersion()
		{
			return Step206.Version;
		}

		public override void DoDownStep(SModel _model)
		{
			// Clear dictionaries before processing
			var openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			Dictionary<string, (string Id, string OriginalModelId, string Type)> partsInModel = new Dictionary<string, (string Id, string OriginalModelId, string type)>();

			var beamsData = openModel.GetElements("Connections;ConnectionData;Beams;BeamData");
			foreach (var beam in beamsData)
			{
				var beamId = beam.GetElementValue("Id");
				var beamOriginalModelId = beam.GetElementValue("OriginalModelId");
				partsInModel[beamId] = (beamId, beamOriginalModelId, "BeamData");
			}

			var platesData = openModel.GetElements("Connections;ConnectionData;Plates;PlateData");
			foreach (var plate in platesData)
			{
				var plateId = plate.GetElementValue("Id");
				var plateOriginalModelId = plate.GetElementValue("OriginalModelId");
				partsInModel[plateId] = (plateId, plateOriginalModelId, "PlateData");
			}


			ProcessGridsForDownStep(openModel.GetElements("Connections;ConnectionData;BoltGrids;BoltGrid"), partsInModel);
			ProcessGridsForDownStep(openModel.GetElements("Connections;ConnectionData;AnchorGrids;AnchorGrid"), partsInModel);
		}



		public override void DoUpStep(SModel _model)
		{
			var openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			Dictionary<string, (string Id, string OriginalModelId, string Type)> beamsInModel = new Dictionary<string, (string Id, string OriginalModelId, string type)>();
			Dictionary<string, (string Id, string OriginalModelId, string Type)> platesInModel = new Dictionary<string, (string Id, string OriginalModelId, string type)>();
			Dictionary<string, (string Id, string OriginalModelId, string Type)> partsInModel = new Dictionary<string, (string Id, string OriginalModelId, string type)>();

			var beamsData = openModel.GetElements("Connections;ConnectionData;Beams;BeamData");
			foreach (var beam in beamsData)
			{
				var beamId = beam.GetElementValue("Id");
				var beamOriginalModelId = beam.TryGetElementValue("OriginalModelId");
				if (!string.IsNullOrEmpty(beamOriginalModelId))
				{
					beamsInModel[beamOriginalModelId] = (beamId, beamOriginalModelId, "BeamData");
					partsInModel[beamOriginalModelId] = (beamId, beamOriginalModelId, "BeamData");
				}
				else
				{
					_logger.LogTrace($"Beam with id {beamId} has not defined OriginalModelId");
				}
			}

			var platesData = openModel.GetElements("Connections;ConnectionData;Plates;PlateData");
			foreach (var plate in platesData)
			{
				var plateId = plate.GetElementValue("Id");
				var plateOriginalModelId = plate.TryGetElementValue("OriginalModelId");

				if (!string.IsNullOrEmpty(plateOriginalModelId))
				{
					platesInModel[plateOriginalModelId] = (plateId, plateOriginalModelId, "PlateData");
					partsInModel[plateOriginalModelId] = (plateId, plateOriginalModelId, "PlateData");
				}
				else
				{
					_logger.LogTrace($"Plate with id {plateId} has not defined OriginalModelId");
				}
			}


			ProcessGridsForUpStep(openModel.GetElements("Connections;ConnectionData;BoltGrids;BoltGrid"), partsInModel);
			ProcessGridsForUpStep(openModel.GetElements("Connections;ConnectionData;AnchorGrids;AnchorGrid"), partsInModel);
		}

		private void ProcessGridsForDownStep(IEnumerable<ISIntermediate> grids, Dictionary<string, (string Id, string OriginalModelId, string Type)> partsInModel)
		{
			foreach (var boltGrid in grids)
			{
				var connectedPartIdsList = boltGrid.CreateElementProperty("ConnectedPartIds").CreateListProperty("ConnectedPartIds");
				var connectedPlatesList = boltGrid.CreateElementProperty("ConnectedPlates").CreateListProperty("ConnectedPlates");

				var connectedPartsId = boltGrid.GetElements("ConnectedParts;ReferenceElement;Id");
				foreach (var connectedPartId in connectedPartsId)
				{
					var partId = connectedPartId.GetElementValue(null);

					if (partsInModel.TryGetValue(partId, out (string Id, string OriginalModelId, string Type) item))
					{
						var part = new SObject() { TypeName = "string" };
						part.ChangeElementValue(item.OriginalModelId);
						connectedPartIdsList.AddElementProperty(part);

						var plate = new SObject() { TypeName = "int" };
						plate.ChangeElementValue(item.Id);
						connectedPlatesList.AddElementProperty(plate);
					}

				}
				boltGrid.RemoveElementProperty("ConnectedParts");
			}
		}

		private void ProcessGridsForUpStep(IEnumerable<ISIntermediate> boltGrids, Dictionary<string, (string Id, string OriginalModelId, string Type)> itemsInModel)
		{
			foreach (var boltGrid in boltGrids)
			{
				var connectedPartsList = boltGrid.CreateElementProperty("ConnectedParts").CreateListProperty("ConnectedParts");
				var connectedIds = boltGrid.GetElements("ConnectedPartIds;string");
				foreach (var connectedId in connectedIds)
				{
					var originalModelid = connectedId.GetElementValue(null);

					if (itemsInModel.TryGetValue(originalModelid, out (string Id, string OriginalModelId, string Type) item))
					{
						var part = new SObject() { TypeName = "ReferenceElement" };
						IRIOMTool.CreateIOMReferenceElement(part, item.Type, item.Id);
						connectedPartsList.AddElementProperty(part);
					}

				}
				boltGrid.RemoveElementProperty("ConnectedPartIds");
				boltGrid.RemoveElementProperty("ConnectedPlates");
			}
		}
	}
}