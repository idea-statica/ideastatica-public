using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal class RamSectionNamed : AbstractRamSection, IIdeaCrossSectionByName
	{
		public IIdeaMaterial Material => GetMaterial();

		public RamSectionNamed(IMaterialFactory materialFactory, RamMemberProperties props)
			: base(materialFactory, props)
		{
		}
	}
}