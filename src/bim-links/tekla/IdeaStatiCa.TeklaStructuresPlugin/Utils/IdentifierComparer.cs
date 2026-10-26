using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utilities
{
	/// <summary>
	/// Identifier Comparer for sorting collection of imported object
	/// </summary>
	public class IdentifierComparer : IComparer<IIdentifier>
	{
		private readonly Dictionary<Type, int> _typePriority = new Dictionary<Type, int>
	{
		{ typeof(IIdeaPlate), 1 },
		{ typeof(IIdeaFoldedPlate), 2 },
		{ typeof(IIdeaConnectedMember), 3 },
	};

		public int Compare(IIdentifier x, IIdentifier y)
		{
			if (x == null || y == null)
			{
				return 0;
			}

			var xType = x.ObjectType;
			var yType = y.ObjectType;

			var xPriority = _typePriority.ContainsKey(xType) ? _typePriority[xType] : int.MaxValue;
			var yPriority = _typePriority.ContainsKey(yType) ? _typePriority[yType] : int.MaxValue;

			return xPriority.CompareTo(yPriority);
		}
	}
}
