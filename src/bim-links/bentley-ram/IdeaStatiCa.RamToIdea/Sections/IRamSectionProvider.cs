using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal interface IRamSectionProvider
	{
		IRamSection GetSection(RamMemberProperties memberProperties);
	}
}