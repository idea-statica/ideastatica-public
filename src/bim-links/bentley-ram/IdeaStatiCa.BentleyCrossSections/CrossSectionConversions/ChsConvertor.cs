using IdeaStatiCa.PluginsTools.CrossSectionConversions;
using System.Collections.Generic;

namespace IdeaStatiCa.BentleyCrossSections.CrossSectionConversions
{
	internal class ChsConvertor : ICssConvertor
	{
		public IEnumerable<Conversion> Conversions => new Conversion[]
		{
			// string HSSP20x0.5 to HSS(ICHS)20x0.5
			new RegexReplace(@"^HSSP[\d.]*[xX][\d.]*$", "HSSP", "HSS(ICHS)"),

			// Creates groups from given css name and using replace reorder these groups ($3$1/$2)
			// string 33.7x3.2CHS to CHS33.7/3.2
			new RegexReplace(@"^([\d.]*)[xX]([\d.]*)(CHS)$", @"$3$1/$2"),
		};
	}
}
