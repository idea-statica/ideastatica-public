using IdeaStatiCa.PluginsTools.CrossSectionConversions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IdeaStatiCa.BentleyCrossSections.CrossSectionConversions
{
	internal class AngleConvertor : ICssConvertor
	{
		public IEnumerable<Conversion> Conversions => new Conversion[]
		{
			// Angle - string L403010 to L40X30X10
			// Angle - string L404010 to L40X10
			new CustomConversion(@"^L(?'dim'\d*)$", ConvertL2L),

			// Angle - string A25x25x3 to 25x25x3EA
			// Angle - string A100x75x6 to 100x75x6UA
			new CustomConversion(@"^A(?'B'[\d.]*)[xX](?'H'[\d.]*)[xX](?'t'[\d.]*)$", ConvertA2EAorUA),

			// Angle - string UA120x120x12 to L120x120x12
			// Angle - string UA50x30x5 to L50x30x5
			new RegexReplace(@"^UA(?'dim'[\d.]*[xX][\d.]*[xX][\d.]*)$", @"L${dim}"),

			// Angle - string L50x50x5 to L50x5
			new CustomConversion(@"^L(?'B'[\d.]*)[xX](?'H'[\d.]*)[xX](?'t'[\d.]*)$", ConvertLx2L),

			// Angle - string L403010 RA - RA meaning switched axes - rotation +90°
			/*TODO*/
		};


		/// <summary>
		/// L50x50x5 to L50x5
		/// </summary>
		/// <param name="text"></param>
		/// <param name="matchRegex"></param>
		/// <returns></returns>
		private static string ConvertLx2L(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("B", out var B) && B.Success &&
				match.Groups.TryGetValue("H", out var H) && H.Success &&
				match.Groups.TryGetValue("t", out var t) && t.Success)
			{
				if (B.Value == H.Value)
				{
					return $"L{B.Value}x{t.Value}";
				}
			}

			return text;
		}

		/// <summary>
		/// A25x25x3 to 25x25x3EA
		/// A100x75x6 to 100x75x6UA
		/// </summary>
		/// <param name="text"></param>
		/// <param name="matchRegex"></param>
		/// <returns></returns>
		private static string ConvertA2EAorUA(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("B", out var B) && B.Success &&
				match.Groups.TryGetValue("H", out var H) && H.Success &&
				match.Groups.TryGetValue("t", out var t) && t.Success)
			{
				return $"{B.Value}x{H.Value}x{t.Value}{(B.Value == H.Value ? "EA" : "UA")}";
			}

			return text;
		}

		/// <summary>
		/// Converts L403010 to L40X30X10 or L404010 to L40X10.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="matchRegex"></param>
		/// <returns></returns>
		private static string ConvertL2L(string text, Regex matchRegex)
		{
			var match = matchRegex.Match(text);
			if (match.Success &&
				match.Groups.TryGetValue("dim", out var dim) && dim.Success)
			{
				var dimString = dim.Value;
				var dims = dimString.Split('0', StringSplitOptions.RemoveEmptyEntries);
				if (dims.Length == 3)
				{
					if (dims[0] == dims[1])
					{
						return $"L{dims[0]}0X{dims[2]}0";
					}

					return $"L{dims[0]}0X{dims[1]}0X{dims[2]}0";
				}
			}

			return text;
		}
	}
}
