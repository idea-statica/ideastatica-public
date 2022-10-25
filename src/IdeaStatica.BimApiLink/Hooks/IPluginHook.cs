using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatica.BimApiLink.Hooks
{
	public interface IPluginHook
	{
		void EnterImport(CountryCode countryCode);

		void ExitImport(CountryCode countryCode);

		void EnterImportSelection(RequestedItemsType requestedType);

		void ExitImportSelection(RequestedItemsType requestedType);
	}
}