using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class NodeImporter : AbstractImporter<IIdeaNode>
	{
		protected override OpenElementId ImportInternal(ImportContext ctx, IIdeaNode node)
		{
			Point3D point = new Point3D()
			{
				Name = node.Name,
				X = node.X,
				Y = node.Y,
				Z = node.Z
			};

			return point;
		}
	}
}