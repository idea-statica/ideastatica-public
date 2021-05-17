using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class NodeImporter : AbstractImporter<IIdeaNode>
	{
		public NodeImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaNode node)
		{
			IdeaVector3D vec = node.Vector;

			Point3D point = new Point3D()
			{
				Name = node.Name,
				X = vec.X,
				Y = vec.Y,
				Z = vec.Z
			};

			return point;
		}
	}
}