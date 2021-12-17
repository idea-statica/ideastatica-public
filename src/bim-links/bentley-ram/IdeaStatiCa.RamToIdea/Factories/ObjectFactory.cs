using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.BimApi;
using RAMDATAACCESSLib;
using System;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class ObjectFactory : IObjectFactory
	{
		private readonly IModel _model;
		private readonly INodes _nodes;

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

		public IIdeaMaterial GetMaterial(EMATERIALTYPES materialType, int uid)
		{
			switch (materialType)
			{
				case EMATERIALTYPES.ESteelMat:
					ISteelMaterial matSteel = _model.GetSteelMaterial(uid);
					return new RamMaterialByName(uid)
					{
						Name = $"Steel {matSteel.dFy}",
						MaterialType = MaterialType.Steel
					};

				case EMATERIALTYPES.EConcreteMat:
					IConcreteMaterial matConcrete = _model.GetConcreteMaterial(uid);
					return new RamMaterialByName(uid)
					{
						Name = $"Concrete {matConcrete.dFpc}",
						MaterialType = MaterialType.Concrete
					};
			}

			throw new ArgumentException(nameof(materialType));
		}
	}
}