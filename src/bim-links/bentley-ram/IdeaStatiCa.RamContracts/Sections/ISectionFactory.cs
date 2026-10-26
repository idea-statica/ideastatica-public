using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Sections
{
	public interface ISectionFactory
	{
		IRamSection GetSection(RamMemberProperties memberProperties);
	}
}