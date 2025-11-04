using IdeaRS.OpenModel.CrossSection;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal interface ISectionPropertiesConverter
	{
		CrossSectionParameter Convert(SteelSectionProperties steelSection);
	}
}