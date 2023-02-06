using IdeaRS.OpenModel;

namespace IdeaStatica.BimApiLink.Hooks
{
	public interface IPluginHookNoScope
	{
		void PreImport(CountryCode countryCode);

		void PostImport(CountryCode countryCode);
	}
}