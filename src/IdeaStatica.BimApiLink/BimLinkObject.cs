using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatica.BimApiLink.Scoping;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink
{
	public class BimLinkObject : ScopeAwareObject
	{
		protected T Get<T>(Identifier<T> identifier)
			where T : IIdeaObject
		{
			return GetMaybe(identifier) ?? throw new InvalidOperationException();
		}

		protected T Get<T>(int id)
			where T : IIdeaObject
		{
			return GetMaybe<T>(id) ?? throw new InvalidOperationException();
		}

		protected T Get<T>(string id)
			where T : IIdeaObject
		{
			return GetMaybe<T>(id) ?? throw new InvalidOperationException();
		}

		protected T? GetMaybe<T>(Identifier<T> identifier)
			where T : IIdeaObject
			=> BimApiImporter.Get(identifier);

		protected T? GetMaybe<T>(int id)
			where T : IIdeaObject
			=> Get(new IntIdentifier<T>(id));

		protected T? GetMaybe<T>(string id)
			where T : IIdeaObject
			=> Get(new StringIdentifier<T>(id));

		protected CountryCode CountryCode
			=> Scope.CountryCode;

		protected object? UserData
			=> Scope.UserData;

		internal IBimApiImporter BimApiImporter
			=> Scope.BimApiImporter;
	}
}