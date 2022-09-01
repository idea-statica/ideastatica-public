using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class AnchorGridImporter : AbstractImporter<IIdeaAnchorGrid>
	{
		public AnchorGridImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaAnchorGrid anchotGrid, ConnectionData connectionData)
		{

			var lcs = anchotGrid.LocalCoordinateSystem as IdeaRS.OpenModel.Geometry3D.CoordSystemByVector;

			AnchorGrid anchorGridIOM = new AnchorGrid()
			{
				Id = 0,
				ConnectedPartIds = anchotGrid.ConnectedParts.Select(cp => cp.Id).ToList(),
				Diameter = anchotGrid.BoltAssembly.Diameter,
				HeadDiameter = anchotGrid.BoltAssembly.HeadDiameter,
				DiagonalHeadDiameter = anchotGrid.BoltAssembly.DiagonalHeadDiameter,
				HeadHeight = anchotGrid.BoltAssembly.HeadHeight,
				BoreHole = anchotGrid.BoltAssembly.BoreHole,
				TensileStressArea = anchotGrid.BoltAssembly.TensileStressArea,
				NutThickness = anchotGrid.BoltAssembly.NutThickness,
				AnchorLen = anchotGrid.BoltAssembly.Lenght,
				Material = anchotGrid.BoltAssembly.Material.Name,
				BoltAssemblyName = anchotGrid.BoltAssembly.Name,
				BoltInteraction = anchotGrid.BoltShearType,
				ShearInThread = anchotGrid.ShearInThread,
				Standard = anchotGrid.BoltAssembly.Standard,
				HoleDiameter = anchotGrid.BoltAssembly.HoleDiameter,
				AxisX = lcs.VecX,
				AxisY = lcs.VecY,
				AxisZ = lcs.VecZ,
				Positions = anchotGrid.Positions.Select(p => ctx.Import(p).Element as Point3D).ToList(),
				Origin = ctx.Import(anchotGrid.Origin).Element as Point3D,
				AnchorType = anchotGrid.AnchorType,
				IsAnchor = true,
				WasherSize = anchotGrid.WasherSize,
				ConcreteBlock = ctx.ImportConnectionItem(anchotGrid.ConcreteBlock, connectionData) as ConcreteBlock,
			};
			(connectionData.AnchorGrids ?? (connectionData.AnchorGrids = new List<AnchorGrid>())).Add(anchorGridIOM);

			//set correct Id
			anchorGridIOM.Id = connectionData.AnchorGrids.Max(b => b.Id) + 1;
			return anchorGridIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaAnchorGrid anchotGrid)
		{
			throw new System.NotImplementedException();
		}
	}
}