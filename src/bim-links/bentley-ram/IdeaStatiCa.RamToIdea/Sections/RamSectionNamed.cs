using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal class RamSectionNamed : AbstractRamSection, IIdeaCrossSectionByName
	{
		public IIdeaMaterial Material => GetMaterial();

		public RamSectionNamed(double height, RamMemberProperties props)
			: base(height, props)
		{
		}
	}
}