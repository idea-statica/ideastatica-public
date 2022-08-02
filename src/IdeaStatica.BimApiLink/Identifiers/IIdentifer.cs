using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public interface IIdentifier : IIdeaPersistenceToken, IEquatable<IIdentifier>
	{
		public Type ObjectType { get; }
	}

	public abstract record Identifier<T> : IIdentifier
		where T : IIdeaObject
	{
		public Type ObjectType => typeof(T);

		public abstract string GetStringId();

		public bool Equals(IIdentifier? other)
			=> other is Identifier<T> id && Equals(id);
	}
}