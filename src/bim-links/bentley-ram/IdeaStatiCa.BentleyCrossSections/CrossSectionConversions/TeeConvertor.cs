using IdeaStatiCa.PluginsTools.CrossSectionConversions;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IdeaStatiCa.BentleyCrossSections.CrossSectionConversions
{
	internal class TeeConvertor : ICssConvertor
	{
		public IEnumerable<Conversion> Conversions => new Conversion[]
		{
			// String WT10.5x83 Tee meaning Tee section from a W21x166 section
			new CustomConversion(@"^WT(?'W'[\d.]*)[xX](?'H'[\d.]*) Tee$", ConvertWT2Tee),

			// String IPE100 Tee meaning Tee section from IPE100
			new CustomConversion(@"^(?'name'...)(?'dim'[\d.]*) Tee$", ConvertTee2Tee),
		};

		private string ConvertTee2Tee(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("name", out var name) && name.Success &&
				match.Groups.TryGetValue("dim", out var dim) && dim.Success)
			{
				return $"Tee {name}{dim}";
			}

			return text;
		}

		private string ConvertWT2Tee(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("W", out var W) && W.Success && double.TryParse(W.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var w) &&
				match.Groups.TryGetValue("H", out var H) && H.Success && double.TryParse(H.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var h))
			{
				return $"Tee W{2 * w}x{2 * h}";
			}

			return text;
		}
	}
}
