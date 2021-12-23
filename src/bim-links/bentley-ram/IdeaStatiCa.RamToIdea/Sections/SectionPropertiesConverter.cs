using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.RamToIdea.Utilities;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal class SectionPropertiesConverter : ISectionPropertiesConverter
	{
		public CrossSectionParameter Convert(SteelSectionProperties steelSection)
		{
			CrossSectionParameter parameter = new CrossSectionParameter();

			switch (steelSection.Shape)
			{
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlWF:
					ConvertEStlWF(steelSection, parameter);
					break;

				default:
					return null;
			}

			return parameter;
		}

		private void ConvertEStlWF(SteelSectionProperties props, CrossSectionParameter parameter)
		{
			CrossSectionFactory.FillCssIarc(parameter,
				props.BfTop.InchesToMeters(),
				props.Depth.InchesToMeters(),
				props.WebT.InchesToMeters(),
				props.TfTop.InchesToMeters(),
				0,
				0,
				0);
		}
	}
}