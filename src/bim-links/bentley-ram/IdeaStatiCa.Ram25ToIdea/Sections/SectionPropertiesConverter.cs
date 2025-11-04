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
					if (steelSection.RolledFlag == RAMDATAACCESSLib.ESTEEL_ROLLED_FLAG.EStlRolled)
					{
						CssFactoryHelper.FillCssIRolled(steelSection, parameter);
					}
					else
					{
						CssFactoryHelper.FillCssISection(steelSection, parameter);
					}

					break;
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlTube:
					CssFactoryHelper.FillCssTube(steelSection, parameter);
					break;
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlPipe:
					CssFactoryHelper.FillCssPipe(steelSection, parameter);
					break;
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlChannel:
					CssFactoryHelper.FillCssChannel(steelSection, parameter);
					break;
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlDoubleL:
					CssFactoryHelper.FillShapeDblLu(steelSection, parameter);
					break;
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlLSection:
					CssFactoryHelper.FillCssAngle(steelSection, parameter);
					break;
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlFlatBar:
					CssFactoryHelper.FillCssRectangle(steelSection, parameter);
					break;
				case RAMDATAACCESSLib.ESTEEL_SEC.EstlRoundBar:
					CssFactoryHelper.FillCssCircle(steelSection, parameter);
					break;
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlTSection:
					CssFactoryHelper.FillCssTsection(steelSection, parameter);
					break;
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlNone:
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlStar:
				case RAMDATAACCESSLib.ESTEEL_SEC.EStlCoreBrace:
				default:
					// Return null and create named cross-section
					return null;
			}

			return parameter;
		}
	}
}