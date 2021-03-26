using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	internal class IIdeaObjectComparer : IEqualityComparer<IIdeaObject>
	{
		public bool Equals(IIdeaObject x, IIdeaObject y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(IIdeaObject obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}