using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System;

namespace IdeaStatica.BimApiLink.BimApi
{
	public abstract class AbstractIdeaObject<T> : BimLinkObject, IIdeaObject, IEquatable<AbstractIdeaObject<T>>
		where T : IIdeaObject
	{
		public string Id => Identifier.GetStringId();

		public virtual string Name { get; set; } = string.Empty;

		public Identifier<T> Identifier { get; }

		protected AbstractIdeaObject(Identifier<T> identifier)
		{
			Identifier = identifier;
		}

		public override int GetHashCode()
			=> Id.GetHashCode();

		public override bool Equals(object obj)
		{
			if (!(obj is AbstractIdeaObject<T> other))
			{
				return false;
			}

			return Equals(other);
		}

		public bool Equals(AbstractIdeaObject<T> other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Id == other.Id;
		}
	}
}