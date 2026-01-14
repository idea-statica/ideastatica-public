using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Factories
{
	public interface IMaterialFactory
	{
		IIdeaMaterial GetMaterial(RamMemberProperties props);
	}
}