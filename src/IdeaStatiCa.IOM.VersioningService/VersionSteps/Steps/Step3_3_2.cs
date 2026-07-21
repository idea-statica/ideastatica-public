using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.IOM.VersioningService.Tools;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	/// <summary>
	/// 3.3.2 - WeldData: consolidate Material(name) + WeldMaterial(ref) into a single Material
	/// reference, and migrate ConnectedPartIds (strings) to ConnectedParts (ReferenceElements).
	/// </summary>
	internal class Step332 : BaseStep
	{
		public Step332(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.3.2");

		public override Version GetVersion() => Step332.Version;

		public override void DoUpStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			// Material lists are model-global.
			var weldMatNameToId = ConnectionRefTool.BuildNameToId(openModel, "MatWelding");
			var steelMatNameToId = ConnectionRefTool.BuildNameToId(openModel, "MatSteel");

			// Connected-part identity (Id/OriginalModelId) is unique only within a connection,
			// so resolve references per connection.
			foreach (var connection in openModel.GetElements("Connections;ConnectionData").OfType<SObject>())
			{
				var partsByOriginalId = new Dictionary<string, (string Id, string Type)>();
				foreach (var plate in ConnectionRefTool.FindAll(connection, "PlateData"))
				{
					AddByOriginalId(partsByOriginalId, plate, "PlateData");
				}
				foreach (var beam in ConnectionRefTool.FindAll(connection, "BeamData"))
				{
					AddByOriginalId(partsByOriginalId, beam, "BeamData");
				}

				foreach (var weld in ConnectionRefTool.FindAll(connection, "WeldData").ToList())
				{
					// Material consolidation into a single reference. The legacy name string could name a
					// welding material or the base steel; the explicit WeldMaterial reference wins when present.
					var name = weld.TryGetElementValue("Material");
					weld.RemoveElementProperty("Material");
					if (weld.Properties.ContainsKey("WeldMaterial"))
					{
						weld.ChangeElementPropertyName("WeldMaterial", "Material");
					}
					else if (!string.IsNullOrEmpty(name) && weldMatNameToId.TryGetValue(name, out var weldMatId))
					{
						IRIOMTool.CreateIOMReferenceElement(weld.CreateElementProperty("Material"), "MatWelding", weldMatId);
					}
					else if (!string.IsNullOrEmpty(name) && steelMatNameToId.TryGetValue(name, out var steelMatId))
					{
						IRIOMTool.CreateIOMReferenceElement(weld.CreateElementProperty("Material"), "MatSteel", steelMatId);
					}
					else if (!string.IsNullOrEmpty(name))
					{
						_logger.LogTrace($"Weld material '{name}' not found in MatWelding/MatSteel; reference left empty");
					}

					// ConnectedPartIds (strings) -> ConnectedParts (ReferenceElements)
					var connectedPartsList = weld.CreateElementProperty("ConnectedParts").CreateListProperty("ConnectedParts");
					foreach (var connectedId in weld.GetElements("ConnectedPartIds;string"))
					{
						var originalModelId = connectedId.GetElementValue(null);
						if (!string.IsNullOrEmpty(originalModelId) && partsByOriginalId.TryGetValue(originalModelId, out var item))
						{
							var part = new SObject() { TypeName = "ReferenceElement" };
							IRIOMTool.CreateIOMReferenceElement(part, item.Type, item.Id);
							connectedPartsList.AddElementProperty(part);
						}
					}
					weld.RemoveElementProperty("ConnectedPartIds");
				}
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

			// Material lists are model-global.
			var weldMatIdToName = ConnectionRefTool.BuildIdToName(openModel, "MatWelding");
			var steelMatIdToName = ConnectionRefTool.BuildIdToName(openModel, "MatSteel");

			foreach (var connection in openModel.GetElements("Connections;ConnectionData").OfType<SObject>())
			{
				// "{Id}_{Type}" -> OriginalModelId, scoped to this connection.
				var originalIdByKey = new Dictionary<string, string>();
				foreach (var plate in ConnectionRefTool.FindAll(connection, "PlateData"))
				{
					AddByKey(originalIdByKey, plate, "PlateData");
				}
				foreach (var beam in ConnectionRefTool.FindAll(connection, "BeamData"))
				{
					AddByKey(originalIdByKey, beam, "BeamData");
				}

				foreach (var weld in ConnectionRefTool.FindAll(connection, "WeldData").ToList())
				{
					// Material reference -> legacy shape. A MatWelding reference is restored as the
					// WeldMaterial reference plus the name string; any other (steel) reference restores
					// just the name string, matching the pre-3.3.2 layout.
					if (weld.Properties.ContainsKey("Material"))
					{
						var matId = ConnectionRefTool.GetReferenceId(weld, "Material");
						var matType = ConnectionRefTool.GetReferenceTypeName(weld, "Material");
						if (matType == "MatWelding")
						{
							weld.ChangeElementPropertyName("Material", "WeldMaterial");
							var name = (matId != null && weldMatIdToName.TryGetValue(matId, out var n)) ? n : string.Empty;
							weld.CreateElementProperty("Material").ChangeElementValue(name);
						}
						else
						{
							var name = (matId != null && steelMatIdToName.TryGetValue(matId, out var n)) ? n : string.Empty;
							weld.RemoveElementProperty("Material");
							weld.CreateElementProperty("Material").ChangeElementValue(name);
						}
					}

					// ConnectedParts (ReferenceElements) -> ConnectedPartIds (strings)
					var connectedPartIdsList = weld.CreateElementProperty("ConnectedPartIds").CreateListProperty("ConnectedPartIds");
					foreach (var part in weld.GetElements("ConnectedParts;ReferenceElement"))
					{
						var partId = part.GetElementValue("Id");
						var partType = part.GetElementValue("TypeName");
						if (originalIdByKey.TryGetValue($"{partId}_{partType}", out var originalModelId))
						{
							var s = new SObject() { TypeName = "string" };
							s.ChangeElementValue(originalModelId);
							connectedPartIdsList.AddElementProperty(s);
						}
					}
					weld.RemoveElementProperty("ConnectedParts");
				}
			}
		}

		private static void AddByOriginalId(Dictionary<string, (string Id, string Type)> map, SObject obj, string type)
		{
			var originalModelId = obj.TryGetElementValue("OriginalModelId");
			var id = obj.TryGetElementValue("Id");
			if (!string.IsNullOrEmpty(originalModelId) && !string.IsNullOrEmpty(id))
			{
				map[originalModelId] = (id, type);
			}
		}

		private static void AddByKey(Dictionary<string, string> map, SObject obj, string type)
		{
			var originalModelId = obj.TryGetElementValue("OriginalModelId");
			var id = obj.TryGetElementValue("Id");
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(originalModelId))
			{
				map[$"{id}_{type}"] = originalModelId;
			}
		}
	}
}
