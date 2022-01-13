using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Geometry;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal interface IObjectFactory
	{
		IIdeaMember1D GetBeam(IBeam beam);

		IIdeaMember1D GetColumn(IColumn column);

		IIdeaMember1D GetHorizontalBrace(IHorizBrace horizBrace);

		IIdeaMember1D GetVerticalBrace(IVerticalBrace verticalBrace);

		IStory GetStory(int uid);
	}
}