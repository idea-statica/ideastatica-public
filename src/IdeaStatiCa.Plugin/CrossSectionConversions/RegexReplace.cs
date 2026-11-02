using System.Text.RegularExpressions;

namespace IdeaStatiCa.PluginsTools.CrossSectionConversions
{
	public class RegexReplace : Conversion
	{
		private readonly string find;
		private readonly string replace;

		public RegexReplace(string pattern, string replace) : this(pattern, string.Empty, replace)
		{
		}

		public RegexReplace(string pattern, string find, string replace) : base(pattern)
		{
			this.find = find;
			this.replace = replace;
		}

		public override string Convert(string text)
		{
			Regex regex = string.IsNullOrEmpty(find) ? MatchRegex : new Regex(find);
			return regex.Replace(text, replace);
		}
	}
}
