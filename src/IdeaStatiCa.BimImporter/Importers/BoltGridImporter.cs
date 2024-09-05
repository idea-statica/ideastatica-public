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
				Name = boltGrid.Name,
				ConnectedParts = boltGrid.ConnectedParts.Select(cp => new ReferenceElement(ctx.ImportConnectionItem(cp, connectionData) as OpenElementId)).ToList(),
				BoltAssembly = ctx.Import(boltGrid.BoltAssembly),
				BoltInteraction = boltGrid.BoltShearType,
				ShearInThread = boltGrid.ShearInThread,
				AxisX = lcs.VecX,
				AxisY = lcs.VecY,
				AxisZ = lcs.VecZ,
				Positions = boltGrid.Positions.Select(p => ctx.Import(p).Element as Point3D).ToList(),
				Origin = ctx.Import(boltGrid.Origin).Element as Point3D,
				Length = boltGrid.Length,
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