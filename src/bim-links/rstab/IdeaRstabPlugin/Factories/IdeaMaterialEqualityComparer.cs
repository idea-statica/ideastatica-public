using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Factories
{
	/// <summary>
	/// Equality comparer for <see cref="IIdeaMaterial"/>. Comparison is done based on Id and Name.
	/// </summary>
	internal class IdeaMaterialEqualityComparer : IEqualityComparer<IIdeaMaterial>
	{
		public bool Equals(IIdeaMaterial x, IIdeaMaterial y)
		{
			return x.Id == y.Id && x.Name == y.Name;
		}

		public int GetHashCode(IIdeaMaterial obj)
		{
			return obj.Id.GetHashCode() + 21 * obj.Name.GetHashCode();
		}
	}
}