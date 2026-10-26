using IdeaRS.OpenModel.CrossSection;

namespace IdeaStatiCa.RamToIdea.Sections
{
	public interface ISectionPropertiesConverter
	{
		CrossSectionParameter Convert(SteelSectionProperties steelSection);
	}
}