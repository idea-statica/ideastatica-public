using IdeaStatiCa.BimApi;
using System;

namespace IdeaStatiCa.BimApiLink.Identifiers
{
	public interface IIdentifier : IIdeaPersistenceToken, IEquatable<IIdentifier>
	{
		Type ObjectType { get; }

		string GetStringId();

		object GetId();
	}
}