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
			var cb = anchotGrid.ConcreteBlock == null ? null : ctx.ImportConnectionItem(anchotGrid.ConcreteBlock, connectionData) as ConcreteBlock;

			AnchorGrid anchorGridIOM = new AnchorGrid()
			{
				Id = 0,
				Name = anchotGrid.Name,
				ConnectedParts = anchotGrid.ConnectedParts.Select(cp => new ReferenceElement(ctx.ImportConnectionItem(cp, connectionData) as OpenElementId)).ToList(),
				BoltAssembly = ctx.Import(anchotGrid.BoltAssembly),
				ShearInThread = anchotGrid.ShearInThread,
				AxisX = lcs.VecX,
				AxisY = lcs.VecY,
				AxisZ = lcs.VecZ,
				Positions = anchotGrid.Positions.Select(p => ctx.Import(p).Element as Point3D).ToList(),
				Origin = ctx.Import(anchotGrid.Origin).Element as Point3D,
				AnchorType = anchotGrid.AnchorType,
				WasherSize = anchotGrid.WasherSize,
				ConcreteBlock = cb,
				AnchoringLength = anchotGrid.AnchoringLength,
				HookLength = anchotGrid.HookLength,
				Length = anchotGrid.Length,
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