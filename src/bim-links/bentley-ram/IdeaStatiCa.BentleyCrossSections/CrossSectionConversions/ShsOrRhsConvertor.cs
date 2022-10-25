using IdeaStatiCa.PluginsTools.CrossSectionConversions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IdeaStatiCa.BentleyCrossSections.CrossSectionConversions
{
	internal class ShsOrRhsConvertor : ICssConvertor
	{
		public IEnumerable<Conversion> Conversions => new Conversion[]
		{
			// SHS - string HSST2x2x0.188 to HSS(Imp)2x2x3/16
			// SHS - string HSST2x2x0.188_A1085 to HSS(Imp)2x2x3/16
			// RHS - string HSST2x2x0.188 to HSS(Imp)2x2x3/16
			// RHS - string HSST2x2x0.188_A1085 to HSS(Imp)2x2x3/16
			// Creates named groups (B, H, t) - dimesions of RHS profile
			new CustomConversion(@"^HSST(?'B'[\d.]*)[xX](?'H'[\d.]*)[xX](?'t'[\d.]*)(.*)$", ConverstHSST2HSS),

			// SHS - string 50x5SHS to SHS50/50/5.0
			new CustomConversion(@"^(?'B'[\d.]*)[xX](?'t'[\d.]*)(?'name'SHS)$", ConvertShs2Shs),

			// RHS - string 50x30x2.6RHS to RHS50/30/2.6
			new CustomConversion(@"^(?'B'[\d.]*)[xX](?'H'[\d.]*)[xX](?'t'[\d.]*)(?'name'RHS)$", ConvertRhs2Rhs),

			// Tube - string TUB16016016.0 to SHS160/160/16.0
			// Tube - string TUB16016016 to SHS160/160/16.0
			// Tube - string TUB90503.2 to RHS90/50/3.2
			// Tube - string TUB120806 to RHS120/80/6.0
			new CustomConversion(@"^TUB(?'dim'[\d.]*)$", ConvertTUB2ShsOrRhs),
		};

		private string ConvertShs2Shs(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("name", out var name) && name.Success &&
				match.Groups.TryGetValue("B", out var B) && B.Success &&
				match.Groups.TryGetValue("t", out var t) && t.Success && double.TryParse(t.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var th))
			{
				return $"{name.Value}{B}/{B}/{th:0.0}";
			}

			return text;
		}

		private string ConvertRhs2Rhs(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("name", out var name) && name.Success &&
				match.Groups.TryGetValue("B", out var B) && B.Success &&
				match.Groups.TryGetValue("H", out var H) && H.Success &&
				match.Groups.TryGetValue("t", out var t) && t.Success && double.TryParse(t.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var th))
			{
				return $"{name.Value}{B}/{H}/{th:0.0}";
			}

			return text;
		}

		private static string ConvertTUB2ShsOrRhs(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("dim", out var dim) && dim.Success)
			{
				var dimString = dim.Value;

				// try parse from the end smth. like '120806' or '90503.2'
				var lastIndexOfdot = dimString.LastIndexOf('.');
				if (lastIndexOfdot < 0)
				{
					lastIndexOfdot = dimString.Length;
				}

				var lastIndexOf0 = dimString.Substring(0, lastIndexOfdot).LastIndexOf('0');
				if (lastIndexOf0 > 0)
				{
					var tString = dimString.Substring(lastIndexOf0 + 1);
					if (double.TryParse(tString, NumberStyles.Number, CultureInfo.InvariantCulture, out var t) && t < 50)
					{
						var bhDimString = dimString.Substring(0, lastIndexOf0 + 1);

						// try to find first zero to split
						var firstIndexOf0 = bhDimString.IndexOf('0');
						if (firstIndexOf0 > 0)
						{
							var B = bhDimString.Substring(0, firstIndexOf0 + 1);
							var H = bhDimString.Substring(firstIndexOf0 + 1, bhDimString.Length - firstIndexOf0 - 1);
							return $"{(B == H ? "SHS" : "RHS")}{B}/{H}/{t:0.0}";
						}
					}
					else
					{
						// TODO, try to find thickness by decimal separator?
					}
				}
			}

			return text;
		}

		private static string ConverstHSST2HSS(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("B", out var B) && match.Groups.TryGetValue("H", out var H) &&
				match.Groups.TryGetValue("t", out var value) && value.Success && double.TryParse(value.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var t))
			{
				var numerator = (int)Math.Round(t * 16, 0);
				return $"HSS(Imp){B.Value}x{H.Value}x{numerator}/16";
			}

			return text;
		}
	}
}
