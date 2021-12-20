using IdeaStatiCa.BimApi;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal interface IObjectFactory
	{
		IIdeaMember1D GetBeam(IBeam beam);

		IIdeaMember1D GetColumn(IColumn column);

		IIdeaMember1D GetHorizontalBrace(IHorizBrace horizBrace);

		IIdeaMember1D GetVerticalBrace(IVerticalBrace verticalBrace);

		IIdeaNode GetNode(INode node);

		IIdeaMaterial GetMaterial(EMATERIALTYPES materialType, int uid);
	}
}