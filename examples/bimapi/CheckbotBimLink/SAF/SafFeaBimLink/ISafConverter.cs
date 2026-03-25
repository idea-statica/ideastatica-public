using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.SAF2IOM.BimApi;

namespace SafFeaBimLink
{
	internal interface ISAFConverter
	{
		ModelBIM ImportConnections(SAFModel model, CountryCode countryCode);

		ModelBIM ImportMember(SAFModel model, CountryCode countryCode);

		IReadOnlyList<ModelBIM> Import(SAFModel model, IEnumerable<BIMItemsGroup> groups, CountryCode countryCode);
	}
}
