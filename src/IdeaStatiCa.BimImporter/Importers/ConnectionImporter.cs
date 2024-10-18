using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ConnectionImporter : AbstractImporter<IIdeaConnectionPoint>
	{
		public ConnectionImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaConnectionPoint connection)
		{
			List<ConnectedMember> connectedMembers = connection.ConnectedMembers?.Where(cm => cm.ConnectedMemberType == IdeaConnectedMemberType.Structural)
				.Select(x => ctx.Import(x).Element as ConnectedMember)
				.ToList();

			ReferenceElement nodeRef = ctx.Import(connection.Node);
			ConnectionPoint connectionPoint = new ConnectionPoint()
			{
				Name = connection.Name,
				ConnectedMembers = connectedMembers,
				Node = nodeRef,
				ProjectFileName = $"Connections/conn-{nodeRef.Id}.ideaCon"
			};


			ConnectionData connectionData = new ConnectionData();

			connectionData.ConnectionPoint = new ReferenceElement(connectionPoint);

			(ctx.OpenModel.Connections ?? (ctx.OpenModel.Connections = new List<ConnectionData>())).Add(connectionData);

			///Add connection items
			connection.ConnectedMembers?.Where(e => e != null).ToList().ForEach(cm => ctx.ImportConnectionItem(cm, connectionData));

			if (connectionData.Beams == null) { connectionData.Beams = new List<BeamData>(); }

			connection.Plates?.Where(e => e != null).ToList().ForEach(p => ctx.ImportConnectionItem(p, connectionData));

			if (connectionData.Plates == null) { connectionData.Plates = new List<PlateData>(); }

			connection.FoldedPlates?.Where(e => e != null).ToList().ForEach(p => ctx.ImportConnectionItem(p, connectionData));

			if (connectionData.FoldedPlates == null) { connectionData.FoldedPlates = new List<FoldedPlateData>(); }

			connection.AnchorGrids?.Where(e => e != null).ToList().ForEach(a => ctx.ImportConnectionItem(a, connectionData));

			if (connectionData.AnchorGrids == null) { connectionData.AnchorGrids = new List<AnchorGrid>(); }

			connection.BoltGrids?.Where(e => e != null).ToList().ForEach(b => ctx.ImportConnectionItem(b, connectionData));

			if (connectionData.BoltGrids == null) { connectionData.BoltGrids = new List<BoltGrid>(); }

			connection.Welds?.Where(e => e != null).ToList().ForEach(w => ctx.ImportConnectionItem(w, connectionData));

			if (connectionData.Welds == null) { connectionData.Welds = new List<WeldData>(); }

			connection.Cuts?.Where(e => e != null).ToList().ForEach(c => ctx.ImportConnectionItem(c, connectionData));

			if (connectionData.CutBeamByBeams == null) { connectionData.CutBeamByBeams = new List<CutBeamByBeamData>(); }

			connectionData.ConcreteBlocks = new List<ConcreteBlockData>();

			connection.PinGrids?.Where(e => e != null).ToList().ForEach(p => ctx.ImportConnectionItem(p, connectionData));

			if (connectionData.PinGrids == null) { connectionData.PinGrids = new List<PinGrid>(); }

			return connectionPoint;
		}
	}
}