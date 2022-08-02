using IdeaRS.OpenModel;

namespace IdeaStatica.BimApiLink
{
	public interface IBimImportContext
	{
		CountryCode CountryCode { get; }
	}

	public class BimImportContext : IBimImportContext
	{
		public CountryCode CountryCode { get; }

		public BimImportContext(CountryCode countryCode)
		{
			CountryCode = countryCode;
		}
	}
}