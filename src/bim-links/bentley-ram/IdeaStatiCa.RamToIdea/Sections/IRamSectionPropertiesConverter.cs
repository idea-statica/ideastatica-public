using IdeaRS.OpenModel.CrossSection;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal interface IRamSectionPropertiesConverter
	{
		CrossSectionParameter Convert(SteelSectionProperties steelSection);
	}
}