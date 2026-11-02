using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.Importers
{
	public interface IBimApiImporter
	{
		T Get<T>(Identifier<T> identifier) where T : IIdeaObject;

		IIdeaObject Get(IIdentifier identifier);

		T Check<T>(Identifier<T> identifier) where T : IIdeaObject;

		IIdeaObject Check(IIdentifier identifier);
	}
}