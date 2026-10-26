using IdeaStatiCa.PluginsTools.CrossSectionConversions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IdeaStatiCa.BentleyCrossSections.CrossSectionConversions
{
	internal class RolledIConvertor : ICssConvertor
	{
		public IEnumerable<Conversion> Conversions => new Conversion[]
		{
			// HP Shapes - string HP8x36 to HP(Imp)8x36
			new RegexReplace(@"^HP[\d.]*[xX][\d.]*$", "HP", "HP(Imp)"),

			// UB Shape - string UB150x14.0 to 150UB14
			// UC Shape - string UC100x14.8 to 100UC14.8
			new CustomConversion(@"^(?'name'UB|UC)(?'H'[\d.]*)[xX](?'B'\d.*)$", ConvertUBorUC),

			// IPE Shape - IPER330 to IPE330R; IPEA330 to IPE330A
			new RegexReplace(@"^IPE(?'name'A|R)(?'H'[\d.]*)$", @"IPE${H}${name}"),

			// HD Shape - HD260x114 to HD260/114
			new RegexReplace(@"^HD[\d.]*[xX][\d.]*$", @"[xX]", @"/"),

			// HE Shape - HE140A to HEA140
			new RegexReplace(@"^HE(?'H'[\d.]*)(?'A'.)$", @"HE${A}${H}"),

			// HE Shape - HEA140AA to HEA140A
			new RegexReplace(@"^HEA(?'H'[\d.]*)(?'A'.)(.)$", @"HEA${H}${A}"),
		};

		private static string ConvertUBorUC(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("name", out var name) && name.Success &&
				match.Groups.TryGetValue("H", out var H) && H.Success && double.TryParse(H.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var h) &&
				match.Groups.TryGetValue("B", out var B) && B.Success && double.TryParse(B.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var b))
			{
				b = Math.Round(b, 1);
				h = Math.Round(h, 1);
				return $"{h}{name}{b}";
			}

			return text;
		}
	}
}
