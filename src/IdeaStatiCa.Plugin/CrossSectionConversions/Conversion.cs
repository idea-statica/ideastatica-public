using System.Text.RegularExpressions;

namespace IdeaStatiCa.PluginsTools.CrossSectionConversions
{
	// Note: [\d.]* - parse real number with dot (.) decimal separator
	// [xX] - separator can be 'x' or 'X'
	// () - noname group
	// (.*) - matches any characters to the unnamed group
	// (?'name') - named group
	// ^ - the begin of the string
	// $ - the end of the string
	// ${name} - usage of named group, when rename (substitute)
	public abstract class Conversion
	{
		protected Conversion(string pattern)
		{
			MatchRegex = new Regex(pattern, RegexOptions.Compiled);
		}

		protected Regex MatchRegex { get; }

		public bool IsMatch(string txt) => MatchRegex.IsMatch(txt);

		public abstract string Convert(string text);
	}
}
