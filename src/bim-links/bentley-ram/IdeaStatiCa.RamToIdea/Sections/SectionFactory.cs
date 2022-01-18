using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Model;
using IdeaStatiCa.RamToIdea.Utilities;
using RAMDATAACCESSLib;
using System;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal class SectionFactory : ISectionFactory
	{
		private readonly ISectionPropertiesConverter _converter;
		private readonly IMemberData1 _memberData;
		private readonly IMaterialFactory _materialFactory;

		public SectionFactory(ISectionPropertiesConverter converter, IMemberData1 memberData, IMaterialFactory materialFactory)
		{
			_converter = converter;
			_memberData = memberData;
			_materialFactory = materialFactory;
		}

		public IRamSection GetSection(RamMemberProperties props)
		{
			if (props.SectionID == 0)
			{
				return CreateNamed(0, props);
			}

			switch (props.MaterialType)
			{
				case EMATERIALTYPES.ESteelMat:
					return GetSteelSection(props);
			}

			return CreateNamed(0, props);
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
				return CreateNamed(steelSection.Depth, props);
			}

			return new RamSectionParametric(
				_materialFactory,
				steelSection.Depth.InchesToMeters(),
				props,
				parameters);
		}

		private IRamSection CreateNamed(double heightInches, RamMemberProperties props)
		{
			return new RamSectionNamed(_materialFactory, heightInches.InchesToMeters(), props);
		}
	}
}