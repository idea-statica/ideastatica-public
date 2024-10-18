using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class PinGridImporter : AbstractImporter<IIdeaPinGrid>
	{
		public PinGridImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaPinGrid pinGrid, ConnectionData connectionData)
		{

			var lcs = pinGrid.LocalCoordinateSystem as IdeaRS.OpenModel.Geometry3D.CoordSystemByVector;

			PinGrid pinGridIOM = new PinGrid()
			{
				Id = 0,
				Name = pinGrid.Name,
				ConnectedParts = pinGrid.ConnectedParts.Select(cp => new ReferenceElement(ctx.ImportConnectionItem(cp, connectionData) as OpenElementId)).ToList(),
				Pin = ctx.Import(pinGrid.Pin),
				AxisX = lcs.VecX,
				AxisY = lcs.VecY,
				AxisZ = lcs.VecZ,
				Positions = pinGrid.Positions.Select(p => ctx.Import(p).Element as Point3D).ToList(),
				Origin = ctx.Import(pinGrid.Origin).Element as Point3D,
			};
			(connectionData.PinGrids ?? (connectionData.PinGrids = new List<PinGrid>())).Add(pinGridIOM);

			//set correct Id
			pinGridIOM.Id = connectionData.BoltGrids.Max(b => b.Id) + 1;
			return pinGridIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaPinGrid boltGrid)
		{
			throw new System.NotImplementedException();
		}
	}
}