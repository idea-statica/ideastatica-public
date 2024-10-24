using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.SAF2IOM.BimApi;
using System;
using System.Collections.Generic;

namespace SafFeaBimLink
{
	internal interface ISAFConverter
	{
		ModelBIM ImportConnections(SAFModel model, CountryCode countryCode);

		ModelBIM ImportMember(SAFModel model, CountryCode countryCode);

		ModelBIM Import(SAFModel model, List<BIMItemId> items, CountryCode countryCode);
	}
}
