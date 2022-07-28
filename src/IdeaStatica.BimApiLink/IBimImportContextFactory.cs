using IdeaRS.OpenModel;

namespace IdeaStatica.BimApiLink
{
	public interface IBimImportContextFactory
	{
		T Create<T>(CountryCode countryCode)
			where T : IBimImportContext;
	}
}