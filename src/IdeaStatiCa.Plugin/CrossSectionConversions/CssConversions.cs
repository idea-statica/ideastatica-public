using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.PluginsTools.CrossSectionConversions
{
	public class CssConversions
	{
		private readonly IEnumerable<Conversion> conversions;
		private readonly IPluginLogger logger;

		internal CssConversions(IEnumerable<Conversion> conversions, IPluginLogger logger = null)
		{
			this.conversions = conversions;
			this.logger = logger;
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
				logger?.LogDebug($"$Unable to parse {originalName} to Idea profile", e);
			}

			return originalName;
		}
	}
}
