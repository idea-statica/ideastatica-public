using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal interface IMaterialFactory
	{
		IIdeaMaterial GetMaterial(RamMemberProperties props);
	}
}