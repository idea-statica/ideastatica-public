using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using Tekla.Structures.Geometry3d;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	public class NodeImporter : BaseImporter<IIdeaNode>
	{
		public NodeImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaNode Create(string id) => new IdeaNode(id)
		{
			Vector = GetNodePosition(Model.GetPoint3D(id))
		};

		private IdeaVector3D GetNodePosition(Point node)
		{
			PlugInLogger.LogDebug($"GetNodePosition create '{node}'");
			return new IdeaVector3D(
						node.X.MilimetersToMeters(),
						node.Y.MilimetersToMeters(),
						node.Z.MilimetersToMeters());
		}
	}
}