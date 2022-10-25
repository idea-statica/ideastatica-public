using System.Collections.Generic;

namespace IdeaStatiCa.PluginsTools.CrossSectionConversions
{
	public class CssConversionBuilder
	{
		private readonly List<Conversion> conversions = new List<Conversion>();

		public CssConversionBuilder Register(ICssConvertor convertor)
		{
			conversions.AddRange(convertor.Conversions);
			return this;
		}

		public CssConversions Build() => new CssConversions(conversions);
	}
}
