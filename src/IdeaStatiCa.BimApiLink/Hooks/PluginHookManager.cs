using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Hooks
{
	internal class PluginHookManager : AbstractHookManager<IPluginHook>, IPluginHook, ISynchronizationHook
	{
		public void ExitImport(CountryCode countryCode)
			=> Invoke(x => x.ExitImport(countryCode));

		public void ExitImportSelection(RequestedItemsType requestedType)
			=> Invoke(x => x.ExitImportSelection(requestedType));

		public void EnterImport(CountryCode countryCode)
			=> Invoke(x => x.EnterImport(countryCode));

		public void EnterImportSelection(RequestedItemsType requestedType)
			=> Invoke(x => x.EnterImportSelection(requestedType));

		public void EnterSynchronization(IReadOnlyCollection<Identifier<IIdeaMember1D>> members)
			=> Invoke(x => (x as ISynchronizationHook)?.EnterSynchronization(members));
	}
}