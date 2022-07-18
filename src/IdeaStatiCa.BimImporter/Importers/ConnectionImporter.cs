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
			List<ConnectedMember> connectedMembers = connection.ConnectedMembers.Where(cm => cm.ConnectedMemberType == IdeaConnectedMemberType.Structural)
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
			connectionData.ConenctionPointId = connectionPoint.Id;
			(ctx.OpenModel.Connections ?? (ctx.OpenModel.Connections = new List<ConnectionData>())).Add(connectionData);

			///Add connection items
			connection.ConnectedMembers.ToList().ForEach(cm => ctx.ImportConnectionItem(cm, connectionData));

			connection.Plates.ToList().ForEach(p => { ctx.ImportConnectionItem(p, connectionData); });

			connection.FoldedPlates.ToList().ForEach(p => { ctx.ImportConnectionItem(p, connectionData); });

			connection.AnchorGrids.ToList().ForEach(a => { ctx.ImportConnectionItem(a, connectionData); });

			connection.BoltGrids.ToList().ForEach(b => { ctx.ImportConnectionItem(b, connectionData); });

			connection.Welds.ToList().ForEach(w => { ctx.ImportConnectionItem(w, connectionData); });

			connection.Cuts.ToList().ForEach(c => { ctx.ImportConnectionItem(c, connectionData); });

			return connectionPoint;
		}
	}
}