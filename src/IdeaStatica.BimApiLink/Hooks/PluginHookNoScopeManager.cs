using IdeaRS.OpenModel;

namespace IdeaStatica.BimApiLink.Hooks
{
	internal class PluginHookNoScopeManager : AbstractHookManager<IPluginHookNoScope>, IPluginHookNoScope
	{
		public void PreImport(CountryCode countryCode)
			=> Invoke(x => x.PreImport(countryCode));

		public void PostImport(CountryCode countryCode)
			=> Invoke(x => x.PostImport(countryCode));
	}
}