using IdeaRS.OpenModel.CrossSection;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal class RamSectionPropertiesConverter : IRamSectionPropertiesConverter
	{
		public CrossSectionParameter Convert(SteelSectionProperties steelSection)
		{
			CrossSectionParameter parameter = new CrossSectionParameter();

			switch (steelSection.Shape)
			{
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlWF:
					ConvertEStlWF(steelSection, parameter);
					break;
			}

			return null;
		}

		private void ConvertEStlWF(SteelSectionProperties props, CrossSectionParameter parameter)
		{
			CrossSectionFactory.FillCssIarc(parameter,
				props.BfTop,
				props.Depth,
				props.WebT,
				props.TfTop,
				0,
				0,
				0);
		}
	}
}