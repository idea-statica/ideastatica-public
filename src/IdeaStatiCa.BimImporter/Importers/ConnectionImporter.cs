using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimImporter.ImportedObjects;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ConnectionImporter : AbstractImporter<Connection>
	{
		protected override OpenElementId ImportInternal(IImportContext ctx, Connection connection)
		{
			List<ConnectedMember> connectedMembers = connection.Members
				.Select(x => ctx.Import(x))
				.Select(x => new ConnectedMember()
				{
					Id = x.Id,
					MemberId = x
				})
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