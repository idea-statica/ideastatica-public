using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal interface ISectionFactory
	{
		IRamSection GetSection(RamMemberProperties memberProperties);
	}
}