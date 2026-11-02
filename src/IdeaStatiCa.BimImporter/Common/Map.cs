using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Common
{
	/// <summary>
	/// Bi-directional dictionary.
	/// </summary>
	internal class Map<TLeft, TRight>
	{
		private readonly Dictionary<TLeft, TRight> _lefts = new Dictionary<TLeft, TRight>();
		private readonly Dictionary<TRight, TLeft> _rights = new Dictionary<TRight, TLeft>();

		public void Add(TLeft left, TRight right)
		{
			_lefts.Add(left, right);
			_rights.Add(right, left);
		}

		public void Set(TLeft left, TRight right)
		{
			_lefts[left] = right;
			_rights[right] = left;
		}

		public TLeft GetLeft(TRight right)
		{
			return _rights[right];
		}

		public TRight GetRight(TLeft left)
		{
			return _lefts[left];
		}

		public bool TryGetLeft(TRight right, out TLeft left)
		{
			if (_rights.TryGetValue(right, out TLeft result))
			{
				left = result;
				return true;
			}

			left = default;
			return false;
		}

		public bool TryGetRight(TLeft left, out TRight right)
		{
			if (_lefts.TryGetValue(left, out TRight result))
			{
				right = result;
				return true;
			}

			right = default;
			return false;
		}

		public void Clear()
		{
			_lefts.Clear();
			_rights.Clear();
		}
	}
}