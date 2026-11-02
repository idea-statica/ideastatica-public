using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Importers;

namespace IdeaStatiCa.BimApiLink.Scoping
{
	public interface IScope
	{
		IBimApiImporter BimApiImporter { get; }

		CountryCode CountryCode { get; }

		object UserData { get; }
	}
}