using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Equality Comparer for <see cref="IIdeaObject"/>. Compares object by their id.
	/// </summary>
	public class IIdeaObjectComparer : IEqualityComparer<IIdeaObject>
	{
		public bool Equals(IIdeaObject x, IIdeaObject y)
		{
			return string.Equals(x.Id, y.Id);
		}

		public int GetHashCode(IIdeaObject obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}