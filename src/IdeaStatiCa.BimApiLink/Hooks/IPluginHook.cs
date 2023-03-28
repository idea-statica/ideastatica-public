using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimApiLink.Hooks
{
	public interface IPluginHook
	{
		void EnterImport(CountryCode countryCode);

		void ExitImport(CountryCode countryCode);

		void EnterImportSelection(RequestedItemsType requestedType);

		void ExitImportSelection(RequestedItemsType requestedType);
	}
}