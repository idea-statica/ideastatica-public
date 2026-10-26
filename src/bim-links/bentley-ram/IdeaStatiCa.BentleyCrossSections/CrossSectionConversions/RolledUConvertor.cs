using IdeaStatiCa.PluginsTools.CrossSectionConversions;
using System.Collections.Generic;

namespace IdeaStatiCa.BentleyCrossSections.CrossSectionConversions
{
	internal class RolledUConvertor : ICssConvertor
	{
		public IEnumerable<Conversion> Conversions => new Conversion[]
		{
			// C Shapes - string C3x6 to C(Imp)3x6
			// MC Shapes - string MC8x8.5 to MC(Imp)8x8.5
			new RegexReplace(@"^(?'name'C|MC)(?'dim'[\d.]*[xX][\d.]*)$", @"${name}(Imp)${dim}"),

			// Channel - string PFC150 to PFC150/75/18
			// Channel - string CH305x102 to CH305/102/46
			/*TODO read mprl from checkbot*/

			// UPN Channel - string UPN100 to UNP100
			new RegexReplace(@"^UPN(?'dim'[\d]*)$", @"UNP${dim}"),
		};
	}
}
