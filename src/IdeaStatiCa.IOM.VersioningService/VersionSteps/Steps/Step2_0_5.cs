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
	internal class Step205 : BaseStep
	{
		private readonly Dictionary<string, string> boltAssemblyDic;
		private readonly Dictionary<string, string> boltGradeDic;

		/// <summary>
		/// Change bolt grids and anchor grids
		/// </summary>
		/// <param name="logger"></param>
		public Step205(IPluginLogger logger) : base(logger)
		{
			boltAssemblyDic = new Dictionary<string, string>();
			boltGradeDic = new Dictionary<string, string>();
		}

		public static Version Version => Version.Parse("2.0.5");

		public override Version GetVersion()
		{
			return Step205.Version;
		}

		public override void DoDownStep(SModel _model)
		{
			// Clear dictionaries before processing
			boltAssemblyDic.Clear();
			boltGradeDic.Clear();

			var openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			var boltAssemblyItemsDic = openModel.GetElements("BoltAssembly;BoltAssembly")
				.ToDictionary(boltAssembly => boltAssembly.GetElementValue("Id"));

			var boltGradesLookup = openModel.GetElements("MatBoltGrade;MaterialBoltGrade")
				.ToDictionary(boltGrade => boltGrade.GetElementValue("Id"), boltGrade => boltGrade.GetElementValue("Name"));

			ProcessBoltGrids(openModel.GetElements("Connections;ConnectionData;BoltGrids;BoltGrid"), boltAssemblyItemsDic, boltGradesLookup);
			ProcessAnchorGrids(openModel.GetElements("Connections;ConnectionData;AnchorGrids;AnchorGrid"), boltAssemblyItemsDic, boltGradesLookup);

			openModel.RemoveElementProperty("BoltAssembly");
			openModel.RemoveElementProperty("MatBoltGrade");
		}

		public override void DoUpStep(SModel _model)
		{
			// Clear dictionaries before processing
			boltAssemblyDic.Clear();
			boltGradeDic.Clear();

			var openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			var boltAssemblyList = openModel.CreateElementProperty("BoltAssembly").CreateListProperty("BoltAssembly");
			var boltGradeList = openModel.CreateElementProperty("MatBoltGrade").CreateListProperty("MatBoltGrade");

			ProcessBoltGridsForUpStep(openModel.GetElements("Connections;ConnectionData;BoltGrids;BoltGrid"), boltAssemblyList, boltGradeList);
			ProcessAnchorGridsForUpStep(openModel.GetElements("Connections;ConnectionData;AnchorGrids;AnchorGrid"), boltAssemblyList, boltGradeList);
		}

		private void ProcessBoltGrids(IEnumerable<ISIntermediate> boltGrids, Dictionary<string, ISIntermediate> boltAssemblyItemsDic, Dictionary<string, string> boltGradesLookup)
		{
			foreach (var boltGrid in boltGrids)
			{
				var boltGridAssemblyId = boltGrid.TakeElementProperty("BoltAssembly")
					.GetElements("Id").FirstOrDefault()?.GetElementValue(null);

				if (boltGridAssemblyId == null)
				{
					_logger.LogWarning("BoltGrid Assembly ID not found.");
					continue;
				}

				if (boltAssemblyItemsDic.TryGetValue(boltGridAssemblyId, out ISIntermediate boltGridAssemblyItem))
				{
					// Copy properties from bolt assembly to bolt grid
					CopyBoltAssemblyProperties(boltGridAssemblyItem, boltGrid);

					boltGrid.RemoveElementProperty("BoreHole");
					boltGrid.ChangeElementPropertyName("Borehole", "BoreHole");

					var boltGradeId = boltGridAssemblyItem.GetElements("BoltGrade;Id")
						.FirstOrDefault()?.GetElementValue(null);

					if (boltGradeId != null && boltGradesLookup.TryGetValue(boltGradeId, out var boltGradeName))
					{
						var di = boltGrid.CreateElementProperty("Material");
						di.ChangeElementValue(boltGradeName);
					}
				}

				boltGrid.CreateElementProperty("IsAnchor", "false");

				IRIOMTool.CopyProperty(boltGrid, boltGrid, "BoreHole", "HoleDiameter");
			}
		}

		private void ProcessAnchorGrids(IEnumerable<ISIntermediate> anchorGrids, Dictionary<string, ISIntermediate> boltAssemblyItemsDic, Dictionary<string, string> boltGradesLookup)
		{
			foreach (var anchorGrid in anchorGrids)
			{
				var anchorGridAssemblyId = anchorGrid.TakeElementProperty("BoltAssembly")
					.GetElements("Id").FirstOrDefault()?.GetElementValue(null);

				if (anchorGridAssemblyId == null)
				{
					_logger.LogWarning("AnchorGrid Assembly ID not found.");
					continue;
				}

				if (boltAssemblyItemsDic.TryGetValue(anchorGridAssemblyId, out ISIntermediate anchorGridAssemblyItem))
				{
					// Copy properties from anchor assembly to anchor grid
					CopyBoltAssemblyProperties(anchorGridAssemblyItem, anchorGrid);

					anchorGrid.RemoveElementProperty("BoreHole");
					anchorGrid.ChangeElementPropertyName("Borehole", "BoreHole");

					var boltGradeId = anchorGridAssemblyItem.GetElements("BoltGrade;Id")
						.FirstOrDefault()?.GetElementValue(null);

					if (boltGradeId != null && boltGradesLookup.TryGetValue(boltGradeId, out var boltGradeName))
					{
						var di = anchorGrid.CreateElementProperty("Material");
						di.ChangeElementValue(boltGradeName);
					}
				}

				IRIOMTool.CopyProperty(anchorGrid, anchorGrid, "AnchoringLength", "AnchorLen");
				anchorGrid.RemoveElementProperty("HookLength");
				anchorGrid.RemoveElementProperty("AnchoringLength");

				anchorGrid.CreateElementProperty("IsAnchor", "true");

				IRIOMTool.CopyProperty(anchorGrid, anchorGrid, "BoreHole", "HoleDiameter");
			}
		}

		private void ProcessBoltGridsForUpStep(IEnumerable<ISIntermediate> boltGrids, SList boltAssemblyList, SList boltGradeList)
		{
			foreach (var boltGrid in boltGrids)
			{
				string boltAssemblyId = GetOrCreateBoltAssembly(boltGrid, boltAssemblyList, boltGradeList);
				var assembly = boltGrid.CreateElementProperty("BoltAssembly");

				RemoveAssemblyPropertiesFromGrid(boltGrid);
				IRIOMTool.CreateIOMReferenceElement(assembly, "BoltAssembly", boltAssemblyId);
			}
		}

		private void ProcessAnchorGridsForUpStep(IEnumerable<ISIntermediate> anchorGrids, SList boltAssemblyList, SList boltGradeList)
		{
			foreach (var anchorGrid in anchorGrids)
			{
				string anchorAssemblyId = GetOrCreateBoltAssembly(anchorGrid, boltAssemblyList, boltGradeList);
				var assembly = anchorGrid.CreateElementProperty("BoltAssembly");

				IRIOMTool.CopyProperty(anchorGrid, anchorGrid, "AnchorLen", "AnchoringLength");

				RemoveAssemblyPropertiesFromGrid(anchorGrid);
				IRIOMTool.CreateIOMReferenceElement(assembly, "BoltAssembly", anchorAssemblyId);
			}
		}

		private string GetOrCreateBoltAssembly(ISIntermediate grid, SList boltAssemblyList, SList boltGradeList)
		{
			string diameter = grid.GetElementValue("Diameter");
			string material = grid.GetElementValue("Material");
			string key = diameter + material;

			if (!boltAssemblyDic.TryGetValue(key, out string assemblyId))
			{
				string gradeId = boltGradeDic.TryGetValue(material, out gradeId)
					? gradeId
					: CreateBoltGrade(boltGradeList, material);

				assemblyId = CreateBoltAssembly(boltAssemblyList, grid, gradeId);
			}

			return assemblyId;
		}

		private string CreateBoltAssembly(SList boltAssemblyList, ISIntermediate grid, string boltGradeId)
		{
			string diameter = grid.GetElementValue("Diameter");
			string material = grid.GetElementValue("Material");

			var newBoltAssembly = new SObject() { TypeName = "BoltAssembly" };
			var boltGradeProperty = newBoltAssembly.CreateElementProperty("BoltGrade");
			IRIOMTool.CreateIOMReferenceElement(boltGradeProperty, "BoltGrade", boltGradeId);
			CopyBoltAssemblyProperties(grid, newBoltAssembly);

			newBoltAssembly.RemoveElementProperty("Borehole");
			newBoltAssembly.ChangeElementPropertyName("BoreHole", "Borehole");

			var idProperty = newBoltAssembly.CreateElementProperty("Id");
			idProperty.ChangeElementValue((boltAssemblyDic.Count + 1).ToString());

			boltAssemblyList.AddElementProperty(newBoltAssembly);
			boltAssemblyDic.Add(diameter + material, (boltAssemblyDic.Count + 1).ToString());
			return (boltAssemblyDic.Count).ToString();
		}

		private string CreateBoltGrade(SList boltGradeList, string material)
		{
			var newBoltGrade = new SObject() { TypeName = "MaterialBoltGrade" };
			newBoltGrade.CreateElementProperty("Name").ChangeElementValue(material);
			newBoltGrade.CreateElementProperty("LoadFromLibrary").ChangeElementValue("true");

			var idProperty = newBoltGrade.CreateElementProperty("Id");
			idProperty.ChangeElementValue((boltGradeList.Count + 1).ToString());

			boltGradeList.AddElementProperty(newBoltGrade);
			boltGradeDic.Add(material, (boltGradeDic.Count + 1).ToString());
			return (boltGradeDic.Count).ToString();
		}

		private static void CopyBoltAssemblyProperties(ISIntermediate source, ISIntermediate target)
		{
			IRIOMTool.CreateProperty(source, target, "Diameter");
			IRIOMTool.CreateProperty(source, target, "Borehole");
			IRIOMTool.CreateProperty(source, target, "BoreHole");
			IRIOMTool.CreateProperty(source, target, "HeadDiameter");
			IRIOMTool.CreateProperty(source, target, "DiagonalHeadDiameter");
			IRIOMTool.CreateProperty(source, target, "HeadHeight");
			IRIOMTool.CreateProperty(source, target, "TensileStressArea");
			IRIOMTool.CreateProperty(source, target, "NutThickness");
		}

		private static void RemoveAssemblyPropertiesFromGrid(ISIntermediate grid)
		{
			grid.RemoveElementProperty("BoltAssemblyRef");
			grid.RemoveElementProperty("IsAnchor");
			grid.RemoveElementProperty("AnchorLen");
			grid.RemoveElementProperty("HoleDiameter");
			grid.RemoveElementProperty("Diameter");
			grid.RemoveElementProperty("HeadDiameter");
			grid.RemoveElementProperty("DiagonalHeadDiameter");
			grid.RemoveElementProperty("HeadHeight");
			grid.RemoveElementProperty("BoreHole");
			grid.RemoveElementProperty("Borehole");
			grid.RemoveElementProperty("TensileStressArea");
			grid.RemoveElementProperty("NutThickness");
			grid.RemoveElementProperty("BoltAssemblyName");
			grid.RemoveElementProperty("Standard");
			grid.RemoveElementProperty("Material");
		}
	}
}