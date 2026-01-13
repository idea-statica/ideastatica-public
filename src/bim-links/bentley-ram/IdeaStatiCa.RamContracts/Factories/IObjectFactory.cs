using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Geometry;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Factories
{
	public interface IObjectFactory
	{
		IIdeaMember1D GetBeam(IBeam beam);

		IIdeaMember1D GetColumn(IColumn column);

		IIdeaMember1D GetHorizontalBrace(IHorizBrace horizBrace);

		IIdeaMember1D GetVerticalBrace(IVerticalBrace verticalBrace);

		IStory GetStory(int uid);

		IIdeaLoadCase GetLoadCase(int uid);

		IIdeaCombiInput GetLoadCombiInput(ILoadCombination combination);

		IIdeaLoadGroup GetLoadGroup(string loadGroupName, IdeaRS.OpenModel.Loading.LoadGroupType type);
	}
}