using IdeaStatiCa.BimApi;
using System;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public abstract class Identifier<T> : IIdentifier
		where T : IIdeaObject
	{
		public Type ObjectType => typeof(T);

		public abstract string GetStringId();

		public virtual bool Equals(IIdentifier other)
		{
			if (!(other is Identifier<T> otherId))
			{
				return false;
			}

			return ObjectType == otherId.ObjectType &&
				GetStringId() == otherId.GetStringId();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Identifier<T> other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ObjectType, GetStringId());
		}
	}
}