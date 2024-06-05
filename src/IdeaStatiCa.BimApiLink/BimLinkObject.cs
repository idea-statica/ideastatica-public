using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Scoping;
using System;

namespace IdeaStatiCa.BimApiLink
{
	public class BimLinkObject : ScopeAwareObject
	{
		protected T Get<T>(Identifier<T> identifier)
			where T : IIdeaObject
		{
			T o = GetMaybe(identifier);
			if (o == null)
			{
				throw new InvalidOperationException();
			}

			return o;
		}

		protected T Get<T>(int id)
			where T : IIdeaObject
		{
			T o = GetMaybe<T>(id);
			if (o == null)
			{
				throw new InvalidOperationException();
			}

			return o;
		}

		protected T Get<T>(string id)
			where T : IIdeaObject
		{
			T o = GetMaybe<T>(id);
			if (o == null)
			{
				throw new InvalidOperationException();
			}

			return o;
		}

		protected T GetMaybe<T>(Identifier<T> identifier)
			where T : IIdeaObject
			=> BimApiImporter.Get(identifier);

		protected T GetMaybe<T>(int id)
			where T : IIdeaObject
			=> GetMaybe(new IntIdentifier<T>(id));

		protected T GetMaybe<T>(string id)
			where T : IIdeaObject
			=> GetMaybe(new StringIdentifier<T>(id));

		protected T CheckMaybe<T>(Identifier<T> identifier)
			where T : IIdeaObject
			=> BimApiImporter.Check(identifier);

		protected T CheckMaybe<T>(int id)
			where T : IIdeaObject
			=> CheckMaybe(new IntIdentifier<T>(id));

		protected T CheckMaybe<T>(string id)
			where T : IIdeaObject
			=> CheckMaybe(new StringIdentifier<T>(id));

		protected CountryCode CountryCode
			=> Scope.CountryCode;

		protected object UserData
			=> Scope.UserData;

		internal IBimApiImporter BimApiImporter
			=> Scope.BimApiImporter;
	}
}