using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatica.BimApiLink.Scoping;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink
{
	public class BimLinkObject : ScopeAwareObject
	{
		protected T2 Get<T2>(Identifier<T2> identifier)
			where T2 : IIdeaObject
			=> BimApiImporter.Get(identifier);

		protected T2 Get<T2>(int id)
			where T2 : IIdeaObject
			=> Get(new IntIdentifier<T2>(id));

		protected T2 Get<T2>(string id)
			where T2 : IIdeaObject
			=> Get(new StringIdentifier<T2>(id));

		protected CountryCode CountryCode
			=> Scope.CountryCode;

		internal IBimApiImporter BimApiImporter
			=> Scope.BimApiImporter;
	}
}