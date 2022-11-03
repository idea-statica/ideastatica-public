using IdeaStatiCa.BimApi;
using System;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public interface IIdentifier : IIdeaPersistenceToken, IEquatable<IIdentifier>
	{
		Type ObjectType { get; }
	}
}