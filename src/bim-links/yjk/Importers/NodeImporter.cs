using yjk.FeaApis;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;
using yjk.ViewModels;

namespace yjk.Importers
{
	internal class NodeImporter : IntIdentifierImporter<IIdeaNode>
	{
		private readonly IFeaGeometryApi geometry;
		private readonly IPluginLogger _logger = AppLogger.Instance;

		public NodeImporter(IFeaGeometryApi geometry)
		{
			this.geometry = geometry;
		}

		public override IIdeaNode Create(int id)
		{
			_logger.LogInformation($"NodeImporter.Create: id={id}");
			var v = GetLocation(id);
			return new IdeaNode(id)
			{
				Vector = v,
			};
		}

		private IdeaVector3D GetLocation(int id)
		{
			IFeaNode feaNode = geometry.GetNode(id);
			return new IdeaVector3D(feaNode.X, feaNode.Y, feaNode.Z);
		}
	}
}