using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Importers;

namespace IdeaStatica.BimApiLink.Scoping
{
	public interface IScope
	{
		IBimApiImporter BimApiImporter { get; }

		CountryCode CountryCode { get; }
	}
}