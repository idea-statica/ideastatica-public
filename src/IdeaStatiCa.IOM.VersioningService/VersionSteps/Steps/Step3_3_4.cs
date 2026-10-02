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
	/// 3.3.4 - Concrete block unification: ConcreteBlockData.Material becomes a MatConcrete reference,
	/// and AnchorGrid.ConcreteBlock (inline simple block) becomes a reference to a ConcreteBlockData
	/// (the box dimensions move onto the referenced ConcreteBlockData via ConcreteBlockBase).
	/// </summary>
	internal class Step334 : BaseStep
	{
		public Step334(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.3.4");

		public override Version GetVersion() => Step334.Version;

		public override void DoUpStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			// 1) Link each AnchorGrid's inline ConcreteBlock to a ConcreteBlockData (per connection).
			foreach (var connection in openModel.GetElements("Connections;ConnectionData").OfType<SObject>())
			{
				var concreteBlocks = connection.GetElements("ConcreteBlocks;ConcreteBlockData").OfType<SObject>().ToList();
				var used = new HashSet<SObject>();
				int maxId = concreteBlocks.Select(GetId).Where(v => v.HasValue).Select(v => v.Value).DefaultIfEmpty(0).Max();

				foreach (var anchorGrid in connection.GetElements("AnchorGrids;AnchorGrid").OfType<SObject>().ToList())
				{
					if (!anchorGrid.Properties.TryGetValue("ConcreteBlock", out var blockObj) || !(blockObj is SObject inlineBlock))
					{
						continue;
					}

					var materialName = inlineBlock.TryGetElementValue("Material");
					var length = inlineBlock.TryGetElementValue("Lenght");
					var width = inlineBlock.TryGetElementValue("Width");
					var height = inlineBlock.TryGetElementValue("Height");

					var target = concreteBlocks.FirstOrDefault(c => !used.Contains(c) && c.TryGetElementValue("Material") == materialName)
						?? concreteBlocks.FirstOrDefault(c => !used.Contains(c));

					string targetId;
					if (target == null)
					{
						// No ConcreteBlockData to attach to - create one (rare; real base-plate models always export it).
						_logger.LogInformation($"UpStep {Version}: AnchorGrid has no ConcreteBlockData in the connection, creating a new one");
						target = new SObject() { TypeName = "ConcreteBlockData" };
						targetId = (++maxId).ToString();
						target.CreateElementProperty("Id").ChangeElementValue(targetId);
						if (!string.IsNullOrEmpty(materialName))
						{
							target.CreateElementProperty("Material").ChangeElementValue(materialName);
						}
						ConnectionRefTool.AddToList(connection, "ConcreteBlocks", "ConcreteBlockData", target);
						concreteBlocks.Add(target);
					}
					else
					{
						targetId = target.TryGetElementValue("Id");
					}

					used.Add(target);
					SetValue(target, "Length", length);
					SetValue(target, "Width", width);
					SetValue(target, "Height", height);

					anchorGrid.RemoveElementProperty("ConcreteBlock");
					IRIOMTool.CreateIOMReferenceElement(anchorGrid.CreateElementProperty("ConcreteBlock"), "ConcreteBlockData", targetId);
				}
			}

			// 2) Convert ConcreteBlockData.Material name string -> MatConcrete reference.
			// A name absent from the (possibly empty) MatConcrete list is materialized as a
			// library-loaded MatConcrete so the reference resolves to the real grade instead of
			// being dropped - old base-plate models carry the block material only on the inline
			// block, not in MatConcrete.
			var nameToId = ConnectionRefTool.BuildNameToId(openModel, "MatConcrete");
			var concreteXsiType = DeriveConcreteXsiType(openModel);
			int maxMatId = nameToId.Values.Select(v => int.TryParse(v, out var n) ? n : 0).DefaultIfEmpty(0).Max();
			foreach (var block in ConnectionRefTool.FindAll(openModel, "ConcreteBlockData").ToList())
			{
				var materialName = block.TryGetElementValue("Material");
				if (!string.IsNullOrEmpty(materialName) && !nameToId.ContainsKey(materialName))
				{
					var newId = (++maxMatId).ToString();
					ConnectionRefTool.MaterializeMaterial(openModel, "MatConcrete", concreteXsiType, materialName, newId);
					nameToId[materialName] = newId;
					_logger.LogInformation($"UpStep {Version}: materialized MatConcrete '{materialName}' (Id {newId}, {concreteXsiType})");
				}
				ConnectionRefTool.StringToReference(block, "Material", "MatConcrete", nameToId, _logger);
			}
		}

		/// <summary>
		/// Pick the MatConcrete .NET type matching the model's design code, derived from the steel
		/// material family already in the model (families are not 1:1 - AISC steel pairs with ACI
		/// concrete, CISC steel with CAN concrete). Falls back to Eurocode.
		/// </summary>
		private static string DeriveConcreteXsiType(ISIntermediate openModel)
		{
			var steel = openModel.GetElements("MatSteel;MatSteel").OfType<SObject>().FirstOrDefault();
			var steelType = (steel != null && steel.Properties.TryGetValue("xsi:type", out var attr) && attr is SAttribute sa)
				? sa.Value
				: null;
			switch (steelType)
			{
				case "MatSteelAISC": return "MatConcreteACI";
				case "MatSteelCISC": return "MatConcreteCAN";
				case "MatSteelAUS": return "MatConcreteAUS";
				case "MatSteelCHN": return "MatConcreteCHN";
				case "MatSteelHKG": return "MatConcreteHKG";
				case "MatSteelIND": return "MatConcreteIND";
				case "MatSteelRUS": return "MatConcreteRUS";
				default: return "MatConcreteEc2"; // MatSteelEc2 and unknown/absent -> Eurocode
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

			// 1) Convert ConcreteBlockData.Material reference -> name string.
			var idToName = ConnectionRefTool.BuildIdToName(openModel, "MatConcrete");
			foreach (var block in ConnectionRefTool.FindAll(openModel, "ConcreteBlockData").ToList())
			{
				ConnectionRefTool.ReferenceToString(block, "Material", idToName, _logger);
			}

			// 2) Rebuild AnchorGrid inline ConcreteBlock from the referenced ConcreteBlockData.
			foreach (var connection in openModel.GetElements("Connections;ConnectionData").OfType<SObject>())
			{
				var concreteBlocks = connection.GetElements("ConcreteBlocks;ConcreteBlockData").OfType<SObject>().ToList();

				foreach (var anchorGrid in connection.GetElements("AnchorGrids;AnchorGrid").OfType<SObject>().ToList())
				{
					var refId = ConnectionRefTool.GetReferenceId(anchorGrid, "ConcreteBlock");
					if (string.IsNullOrEmpty(refId))
					{
						continue;
					}

					var target = concreteBlocks.FirstOrDefault(c => c.TryGetElementValue("Id") == refId);
					if (target == null)
					{
						continue;
					}

					var materialName = target.TryGetElementValue("Material");
					var length = target.TryGetElementValue("Length");
					var width = target.TryGetElementValue("Width");
					var height = target.TryGetElementValue("Height");

					anchorGrid.RemoveElementProperty("ConcreteBlock");
					var inlineBlock = anchorGrid.CreateElementProperty("ConcreteBlock");
					SetValue(inlineBlock, "Lenght", length);
					SetValue(inlineBlock, "Width", width);
					SetValue(inlineBlock, "Height", height);
					SetValue(inlineBlock, "Material", materialName);

					// The box dimensions belong on the inline block only, remove them from ConcreteBlockData.
					target.RemoveElementProperty("Length");
					target.RemoveElementProperty("Width");
					target.RemoveElementProperty("Height");
				}
			}
		}

		private static int? GetId(SObject obj)
		{
			return int.TryParse(obj.TryGetElementValue("Id"), out var id) ? id : (int?)null;
		}

		private static void SetValue(SObject owner, string property, string value)
		{
			if (value == null)
			{
				return;
			}
			owner.RemoveElementProperty(property);
			owner.CreateElementProperty(property).ChangeElementValue(value);
		}
	}
}
