using System;
using System.Text.RegularExpressions;

namespace IdeaStatiCa.PluginsTools.CrossSectionConversions
{
	public class CustomConversion : Conversion
	{
		private readonly Func<string, Regex, string> func;

		public CustomConversion(string pattern, Func<string, Regex, string> func) : base(pattern)
		{
			this.func = func;
		}

		public override string Convert(string text)
		{
			return func.Invoke(text, MatchRegex);
		}
	}
}
