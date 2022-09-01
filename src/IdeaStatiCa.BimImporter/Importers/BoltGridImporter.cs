using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class BoltGridImporter : AbstractImporter<IIdeaBoltGrid>
	{
		public BoltGridImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaBoltGrid boltGrid, ConnectionData connectionData)
		{

			var lcs = boltGrid.LocalCoordinateSystem as IdeaRS.OpenModel.Geometry3D.CoordSystemByVector;

			BoltGrid boltGridIOM = new BoltGrid()
			{
				Id = 0,
				ConnectedPartIds = boltGrid.ConnectedParts.Select(cp => cp.Id).ToList(),
				Diameter = boltGrid.BoltAssembly.Diameter,
				HeadDiameter = boltGrid.BoltAssembly.HeadDiameter,
				DiagonalHeadDiameter = boltGrid.BoltAssembly.DiagonalHeadDiameter,
				HeadHeight = boltGrid.BoltAssembly.HeadHeight,
				BoreHole = boltGrid.BoltAssembly.BoreHole,
				TensileStressArea = boltGrid.BoltAssembly.TensileStressArea,
				NutThickness = boltGrid.BoltAssembly.NutThickness,
				AnchorLen = boltGrid.BoltAssembly.Lenght,
				Material = boltGrid.BoltAssembly.Material.Name,
				BoltAssemblyName = boltGrid.BoltAssembly.Name,
				BoltInteraction = boltGrid.BoltShearType,
				ShearInThread = boltGrid.ShearInThread,
				Standard = boltGrid.BoltAssembly.Standard,
				HoleDiameter = boltGrid.BoltAssembly.HoleDiameter,
				AxisX = lcs.VecX,
				AxisY = lcs.VecY,
				AxisZ = lcs.VecZ,
				Positions = boltGrid.Positions.Select(p => ctx.Import(p).Element as Point3D).ToList(),
				Origin = ctx.Import(boltGrid.Origin).Element as Point3D,
			};
			(connectionData.BoltGrids ?? (connectionData.BoltGrids = new List<BoltGrid>())).Add(boltGridIOM);

			//set correct Id
			boltGridIOM.Id = connectionData.BoltGrids.Max(b => b.Id) + 1;
			return boltGridIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaBoltGrid boltGrid)
		{
			throw new System.NotImplementedException();
		}
	}
}