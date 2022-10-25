using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatica.BimApiLink.Hooks
{
	internal class PluginHookManager : AbstractHookManager<IPluginHook>, IPluginHook
	{
		public void ExitImport(CountryCode countryCode)
			=> Invoke(x => x.ExitImport(countryCode));

		public void ExitImportSelection(RequestedItemsType requestedType)
			=> Invoke(x => x.ExitImportSelection(requestedType));

		public void EnterImport(CountryCode countryCode)
			=> Invoke(x => x.EnterImport(countryCode));

		public void EnterImportSelection(RequestedItemsType requestedType)
			=> Invoke(x => x.EnterImportSelection(requestedType));
	}
}