using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Importers
{
	public interface IImporter
	{
		T Create<T>(Identifier<T> identifier)
			where T : IIdeaObject;

		IIdeaObject Create(IIdentifier identifier);
	}

	public interface IImporter<T> : IImporter
		where T : IIdeaObject
	{
		T Create(Identifier<T> identifier);
	}
}