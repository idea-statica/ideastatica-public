using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.RamToIdea.Model;
using RAMDATAACCESSLib;
using System;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal class SectionFactory : ISectionFactory
	{
		private readonly ISectionPropertiesConverter _converter;
		private readonly IMemberData1 _memberData;

		public SectionFactory(ISectionPropertiesConverter converter, IMemberData1 memberData)
		{
			_converter = converter;
			_memberData = memberData;
		}

		public IRamSection GetSection(RamMemberProperties props)
		{
			switch (props.MaterialType)
			{
				case EMATERIALTYPES.ESteelMat:
					return GetSteelSection(props);
			}

			return new RamSectionNamed(0, props);
		}

		private IRamSection GetSteelSection(RamMemberProperties props)
		{
			SteelSectionProperties steelSection = new SteelSectionProperties();

			_memberData.GetSteelMemberSectionDimProps(
				props.MemberUID,
				ref steelSection.Shape,
				ref steelSection.StrSize,
				ref steelSection.BfTop,
				ref steelSection.BFBot,
				ref steelSection.TfTop,
				ref steelSection.TFBot,
				ref steelSection.kTop,
				ref steelSection.kBot,
				ref steelSection.Depth,
				ref steelSection.WebT,
				ref steelSection.Cw,
				ref steelSection.J,
				ref steelSection.RolledFlag,
				ref steelSection.Zx,
				ref steelSection.Zy,
				ref steelSection.Sxtop,
				ref steelSection.Sxbot,
				ref steelSection.Sy,
				ref steelSection.Imaj,
				ref steelSection.Imin,
				ref steelSection.Area);

			CrossSectionParameter parameters = _converter.Convert(steelSection);

			if (parameters is null)
			{
				return new RamSectionNamed(steelSection.Depth, props);
			}

			return new RamSectionParametric(
				steelSection.Depth,
				props,
				parameters);
		}
	}
}