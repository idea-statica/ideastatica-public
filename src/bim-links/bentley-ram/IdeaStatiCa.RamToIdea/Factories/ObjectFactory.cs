using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.BimApi;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class ObjectFactory : IObjectFactory
	{
		private IModel _model;
		private INodes _nodes;

		public ObjectFactory(IModel model)
		{
			_model = model;
			_nodes = _model.GetFrameAnalysisNodes();
		}

		public IIdeaMember1D GetBeam(IBeam beam)
		{
			return new RamMemberBeam(this, _nodes, beam);
		}

		public IIdeaMember1D GetColumn(IColumn column)
		{
			return new RamMemberColumn(this, _nodes, column);
		}

		public IIdeaMember1D GetHorizontalBrace(IHorizBrace horizBrace)
		{
			return new RamMemberHorizontalBrace(this, _nodes, horizBrace);
		}

		public IIdeaMember1D GetVerticalBrace(IVerticalBrace verticalBrace)
		{
			return new RamMemberVerticalBrace(this, _nodes, verticalBrace);
		}

		public IIdeaNode GetNode(INode node)
		{
			return new RamNode(node);
		}
	}
}