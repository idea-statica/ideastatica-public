using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.PluginsTools.CrossSectionConversions
{
	public class CssConversions
	{
		private readonly IEnumerable<Conversion> conversions;

		internal CssConversions(IEnumerable<Conversion> conversions)
		{
			this.conversions = conversions;
		}

		public string ConvertCrossSectionName(string originalName)
		{
			try
			{
				var rgx = conversions.FirstOrDefault(x => x.IsMatch(originalName));
				if (rgx != null)
				{
					return rgx.Convert(originalName) ?? originalName;
				}
			}
			catch (Exception e)
			{
				//ideaLogger.LogDebug($"$Unable to parse {originalName} to Idea profile", e);
				System.Diagnostics.Debug.WriteLine($"$Unable to parse {originalName} to Idea profile", e);
			}

			return originalName;
		}
	}
}
