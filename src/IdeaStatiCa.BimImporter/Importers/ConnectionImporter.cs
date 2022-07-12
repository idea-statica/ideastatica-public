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
			List<ConnectedMember> connectedMembers = connection.ConnectedMembers
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

			return connectionPoint;
		}
	}
}