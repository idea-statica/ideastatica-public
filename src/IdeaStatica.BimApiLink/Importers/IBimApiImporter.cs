using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Importers
{
	public interface IBimApiImporter
	{
		T Get<T>(Identifier<T> identifier) where T : IIdeaObject;

		IIdeaObject Get(IIdentifier identifier);
	}
}