using System.Collections.Generic;

namespace IdeaStatiCa.PluginsTools.CrossSectionConversions
{
	public interface ICssConvertor
	{
		IEnumerable<Conversion> Conversions { get; }
	}
}
