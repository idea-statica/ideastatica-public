using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BentleyCrossSections.CrossSectionConversions;
using IdeaStatiCa.PluginsTools.CrossSectionConversions;
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
		private static readonly Lazy<CssConversions> conversions = new Lazy<CssConversions>(() => BuildConvertions());

		public SectionFactory(ISectionPropertiesConverter converter, IMemberData1 memberData, IMaterialFactory materialFactory)
		{
			_converter = converter;
			_memberData = memberData;
			_materialFactory = materialFactory;
		}

		private static CssConversions Conversions => conversions.Value;

		public IRamSection GetSection(RamMemberProperties props)
		{
			if (props.SectionID == 0 && string.IsNullOrEmpty(props.SectionLabel))
			{
				return CreateNamed(props);
			}

			switch (props.MaterialType)
			{
				case EMATERIALTYPES.ESteelMat:
					return GetSteelSection(props);
			}

			return CreateNamed(props);
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
				return CreateNamed(props);
			}

			return new RamSectionParametric(
				_materialFactory,
				props,
				parameters);
		}

		private IRamSection CreateNamed(RamMemberProperties props)
		{
			props.SectionLabel = ConvertCrossSectionName(props.SectionLabel);
			return new RamSectionNamed(_materialFactory, props);
		}

		private static string ConvertCrossSectionName(string ramName)
		{
			return Conversions.ConvertCrossSectionName(ramName);
		}

		private static CssConversions BuildConvertions()
		{
			var builder = new CssConversionBuilder();
			builder
				.Register(new RolledIConvertor())
				.Register(new RolledUConvertor())
				.Register(new ChsConvertor())
				.Register(new ShsOrRhsConvertor())
				.Register(new AngleConvertor())
				.Register(new TeeConvertor());

			return builder.Build();
		}
	}
}