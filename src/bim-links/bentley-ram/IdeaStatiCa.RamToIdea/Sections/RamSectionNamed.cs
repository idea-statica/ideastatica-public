using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal class RamSectionNamed : AbstractRamSection, IIdeaCrossSectionByName
	{
		public IIdeaMaterial Material => GetMaterial();

		public RamSectionNamed(IObjectFactory objectFactory, double height, RamMemberProperties props)
			: base(objectFactory, height, props)
		{
		}
	}
}