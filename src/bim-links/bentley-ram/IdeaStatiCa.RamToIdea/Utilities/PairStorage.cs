using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.Utilities
{
	internal class PairStorage<TLeft, TRight> : IEnumerable<(TLeft, TRight)>
	{
		private readonly Dictionary<TLeft, HashSet<TRight>> _left2Right;
		private readonly Dictionary<TRight, HashSet<TLeft>> _right2Left;

		private class PairGroup<TKey, TElements> : IGrouping<TKey, TElements>
		{
			public TKey Key { get; }
			private readonly IEnumerable<TElements> _elements;

			public PairGroup(TKey key, IEnumerable<TElements> elements)
			{
				Key = key;
				_elements = elements;
			}

			public IEnumerator<TElements> GetEnumerator() => _elements.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();
		}

		public PairStorage()
		{
			_left2Right = new Dictionary<TLeft, HashSet<TRight>>();
			_right2Left = new Dictionary<TRight, HashSet<TLeft>>();
		}

		public PairStorage(IEqualityComparer<TLeft> leftComparer, IEqualityComparer<TRight> rightComparer)
		{
			_left2Right = new Dictionary<TLeft, HashSet<TRight>>(leftComparer);
			_right2Left = new Dictionary<TRight, HashSet<TLeft>>(rightComparer);
		}

		public void Clear()
		{
			_left2Right.Clear();
			_right2Left.Clear();
		}

		public void Add(TLeft left, TRight right)
		{
			GetOrCreate(_left2Right, left).Add(right);
			GetOrCreate(_right2Left, right).Add(left);
		}

		public void Add(TLeft left, IEnumerable<TRight> rights)
		{
			AddMany(_left2Right, _right2Left, left, rights);
		}

		public void Add(IEnumerable<TLeft> left, TRight rights)
		{
			AddMany(_right2Left, _left2Right, rights, left);
		}

		public IEnumerable<TLeft> GetLefts()
		{
			return _left2Right.Keys;
		}

		public IEnumerable<TLeft> GetLefts(TRight right)
		{
			return _right2Left[right];
		}

		public IEnumerable<TRight> GetRights()
		{
			return _right2Left.Keys;
		}

		public IEnumerable<TRight> GetRights(TLeft left)
		{
			return _left2Right[left];
		}

		public void RemoveLeft(TLeft left)
		{
			Remove(_left2Right, _right2Left, left);
		}

		public void RemoveRight(TRight right)
		{
			Remove(_right2Left, _left2Right, right);
		}

		public void ReplaceLeft(TLeft left, IEnumerable<TRight> rights)
		{
			Replace(_left2Right, _right2Left, left, rights);
		}

		public void ReplaceRight(TRight right, IEnumerable<TLeft> lefts)
		{
			Replace(_right2Left, _left2Right, right, lefts);
		}

		public IEnumerable<IGrouping<TLeft, TRight>> EnumerateByLeft()
		{
			return _left2Right.Select(x => new PairGroup<TLeft, TRight>(x.Key, x.Value));
		}

		public IEnumerable<IGrouping<TRight, TLeft>> EnumerateByRight()
		{
			return _right2Left.Select(x => new PairGroup<TRight, TLeft>(x.Key, x.Value));
		}

		public bool Contains(TLeft left, TRight right)
		{
			if (_left2Right.TryGetValue(left, out HashSet<TRight> rights))
			{
				return rights.Contains(right);
			}

			return false;
		}

		public IEnumerator<(TLeft, TRight)> GetEnumerator()
		{
			return EnumerateAll().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return EnumerateAll().GetEnumerator();
		}

		private IEnumerable<(TLeft, TRight)> EnumerateAll()
		{
			foreach (KeyValuePair<TLeft, HashSet<TRight>> leftRights in _left2Right)
			{
				foreach (TRight right in leftRights.Value)
				{
					yield return (leftRights.Key, right);
				}
			}
		}

		private static HashSet<T2> GetOrCreate<T1, T2>(Dictionary<T1, HashSet<T2>> col, T1 key)
		{
			if (!col.TryGetValue(key, out HashSet<T2> set))
			{
				set = new HashSet<T2>();
				col.Add(key, set);
			}

			return set;
		}

		private static void Remove<T1, T2>(Dictionary<T1, HashSet<T2>> col1, Dictionary<T2, HashSet<T1>> col2, T1 key)
		{
			if (!col1.ContainsKey(key))
			{
				return;
			}

			foreach (T2 val in col1[key])
			{
				col2[val].Remove(key);
			}

			col1.Remove(key);
		}

		private static void AddMany<T1, T2>(Dictionary<T1, HashSet<T2>> col1, Dictionary<T2, HashSet<T1>> col2, T1 key, IEnumerable<T2> values)
		{
			HashSet<T2> currentValues = GetOrCreate(col1, key);
			foreach (T2 val in values)
			{
				currentValues.Add(val);
				GetOrCreate(col2, val).Add(key);
			}
		}

		private static void Replace<T1, T2>(Dictionary<T1, HashSet<T2>> col1, Dictionary<T2, HashSet<T1>> col2, T1 key, IEnumerable<T2> values)
		{
			if (!col1.ContainsKey(key))
			{
				AddMany(col1, col2, key, values);
				return;
			}

			HashSet<T2> current = col1[key];
			HashSet<T2> removeFrom = new HashSet<T2>(current);

			foreach (T2 val in values)
			{
				if (!current.Contains(val))
				{
					current.Add(val);
					GetOrCreate(col2, val).Add(key);
				}

				removeFrom.Remove(val);
			}

			foreach (T2 val in removeFrom)
			{
				current.Remove(val);
				col2[val].Remove(key);
			}
		}
	}
}
