using System;
using System.Collections;
using System.Collections.Generic;

namespace CI.Common.MultiMethods
{
	public class DispatchTable
	{
		private int _argumentCount;
		private bool _is32BitPlatform;
		private volatile IDictionary _matches; // copy on write SortedList<types[], MethodInvoker> optimized for #args

		public DispatchTable(int argumentCount, bool is32BitPlatform)
		{
			_argumentCount = argumentCount;
			_is32BitPlatform = is32BitPlatform;

			if (_is32BitPlatform)
			{
				if (_argumentCount == 1)
					_matches = new SortedList<IntKey1, MethodInvoker>();
				else if (_argumentCount == 2)
					_matches = new SortedList<IntKey2, MethodInvoker>();
				else if (_argumentCount == 3)
					_matches = new SortedList<IntKey3, MethodInvoker>();
				else if (_argumentCount == 4)
					_matches = new SortedList<IntKey4, MethodInvoker>();
			}
			else
			{
				if (_argumentCount == 1)
					_matches = new SortedList<LongKey1, MethodInvoker>();
				else if (_argumentCount == 2)
					_matches = new SortedList<LongKey2, MethodInvoker>();
				else if (_argumentCount == 3)
					_matches = new SortedList<LongKey3, MethodInvoker>();
				else if (_argumentCount == 4)
					_matches = new SortedList<LongKey4, MethodInvoker>();
			}

			if (_matches == null)
			{
				throw new ArgumentException("Expecting a value between 1 and 4", "argumentCount");
			}
		}

		public int Size
		{
			get { return _matches.Count; }
		}

		public MethodInvoker Match(object[] args)
		{
			if (_is32BitPlatform)
				return MatchIntKey(args);
			else
				return MatchLongKey(args);
		}

		public void Add(object[] args, MethodInvoker target)
		{
			if (_is32BitPlatform)
			{
				AddIntKey(args, target);
			}
			else
			{
				AddLongKey(args, target);
			}
		}

		// non thread safe
		internal void InternalAdd(Type[] args, MethodInvoker target)
		{
			if (_is32BitPlatform)
			{
				AddIntKey_InPlace(args, target);
			}
			else
			{
				AddLongKey_InPlace(args, target);
			}
		}

		#region Add Key

		// non thread safe
		private void AddKey_InPlace<TKey>(TKey key, MethodInvoker value)
		{
			SortedList<TKey, MethodInvoker> list = _matches as SortedList<TKey, MethodInvoker>;

			if (!list.ContainsKey(key))
			{
				list[key] = value;
			}
		}

		// thread safe: updates a copy of _matches and then assigns copy to _matches
		private void AddKey<TKey>(TKey key, MethodInvoker target)
			where TKey : IComparable<TKey>
		{
			SortedList<TKey, MethodInvoker> current = (SortedList<TKey, MethodInvoker>)_matches;
			SortedList<TKey, MethodInvoker> copy =
				new SortedList<TKey, MethodInvoker>(current.Count + 1);

			bool added = false;

			foreach (KeyValuePair<TKey, MethodInvoker> pair in current)
			{
				int c = 0;

				if (!added && (c = key.CompareTo(pair.Key)) <= 0)
				{
					added = true;
					if (c != 0)
					{
						copy.Add(key, target);
					}
				}

				copy.Add(pair.Key, pair.Value);
			}

			if (!added)
			{
				copy.Add(key, target);
			}

			_matches = copy;
		}

		#endregion

		#region Int32 keys

		private MethodInvoker MatchIntKey(object[] args)
		{
			MethodInvoker target = null;

			switch (_argumentCount)
			{
				case 1:
					IntKey1 key1 = new IntKey1(args);
					(_matches as SortedList<IntKey1, MethodInvoker>).TryGetValue(key1, out target);
					break;
				case 2:
					IntKey2 key2 = new IntKey2(args);
					(_matches as SortedList<IntKey2, MethodInvoker>).TryGetValue(key2, out target);
					break;
				case 3:
					IntKey3 key3 = new IntKey3(args);
					(_matches as SortedList<IntKey3, MethodInvoker>).TryGetValue(key3, out target);
					break;
				default:
					IntKey4 key4 = new IntKey4(args);
					(_matches as SortedList<IntKey4, MethodInvoker>).TryGetValue(key4, out target);
					break;
			}

			return target;
		}

		private void AddIntKey_InPlace(Type[] args, MethodInvoker target)
		{
			switch (_argumentCount)
			{
				case 1:
					IntKey1 key1 = new IntKey1(args);
					AddKey_InPlace(key1, target);
					break;
				case 2:
					IntKey2 key2 = new IntKey2(args);
					AddKey_InPlace(key2, target);
					break;
				case 3:
					IntKey3 key3 = new IntKey3(args);
					AddKey_InPlace(key3, target);
					break;
				default:
					IntKey4 key4 = new IntKey4(args);
					AddKey_InPlace(key4, target);
					break;
			}
		}

		private void AddIntKey(object[] args, MethodInvoker target)
		{
			switch (_argumentCount)
			{
				case 1:
					IntKey1 key1 = new IntKey1(args);
					AddKey(key1, target);
					break;
				case 2:
					IntKey2 key2 = new IntKey2(args);
					AddKey(key2, target);
					break;
				case 3:
					IntKey3 key3 = new IntKey3(args);
					AddKey(key3, target);
					break;
				default:
					IntKey4 key4 = new IntKey4(args);
					AddKey(key4, target);
					break;
			}
		}

		private struct IntKey1 : IComparable<IntKey1>
		{
			private int _arg1Type;

			public IntKey1(object[] args)
			{
				_arg1Type = args[0].GetType().TypeHandle.Value.ToInt32();
			}

			public IntKey1(Type[] args)
			{
				_arg1Type = args[0].TypeHandle.Value.ToInt32();
			}

			public int CompareTo(IntKey1 other)
			{
				return _arg1Type.CompareTo(other._arg1Type);
			}
		}

		private struct IntKey2 : IComparable<IntKey2>
		{
			private int _arg1Type;
			private int _arg2Type;

			public IntKey2(object[] args)
			{
				_arg1Type = args[0].GetType().TypeHandle.Value.ToInt32();
				_arg2Type = args[1].GetType().TypeHandle.Value.ToInt32();
			}

			public IntKey2(Type[] args)
			{
				_arg1Type = args[0].TypeHandle.Value.ToInt32();
				_arg2Type = args[1].TypeHandle.Value.ToInt32();
			}

			public int CompareTo(IntKey2 other)
			{
				int result = _arg1Type.CompareTo(other._arg1Type);

				if (result != 0)
					return result;

				return _arg2Type.CompareTo(other._arg2Type);
			}
		}

		private struct IntKey3 : IComparable<IntKey3>
		{
			private int _arg1Type;
			private int _arg2Type;
			private int _arg3Type;

			public IntKey3(object[] args)
			{
				_arg1Type = args[0].GetType().TypeHandle.Value.ToInt32();
				_arg2Type = args[1].GetType().TypeHandle.Value.ToInt32();
				_arg3Type = args[2].GetType().TypeHandle.Value.ToInt32();
			}

			public IntKey3(Type[] args)
			{
				_arg1Type = args[0].TypeHandle.Value.ToInt32();
				_arg2Type = args[1].TypeHandle.Value.ToInt32();
				_arg3Type = args[2].TypeHandle.Value.ToInt32();
			}

			public int CompareTo(IntKey3 other)
			{
				int result = _arg1Type.CompareTo(other._arg1Type);

				if (result != 0)
					return result;

				result = _arg2Type.CompareTo(other._arg2Type);

				if (result != 0)
					return result;

				return _arg3Type.CompareTo(other._arg3Type);
			}
		}

		private struct IntKey4 : IComparable<IntKey4>
		{
			private int _arg1Type;
			private int _arg2Type;
			private int _arg3Type;
			private int _arg4Type;

			public IntKey4(object[] args)
			{
				_arg1Type = args[0].GetType().TypeHandle.Value.ToInt32();
				_arg2Type = args[1].GetType().TypeHandle.Value.ToInt32();
				_arg3Type = args[2].GetType().TypeHandle.Value.ToInt32();
				_arg4Type = args[3].GetType().TypeHandle.Value.ToInt32();
			}

			public IntKey4(Type[] args)
			{
				_arg1Type = args[0].TypeHandle.Value.ToInt32();
				_arg2Type = args[1].TypeHandle.Value.ToInt32();
				_arg3Type = args[2].TypeHandle.Value.ToInt32();
				_arg4Type = args[3].TypeHandle.Value.ToInt32();
			}

			public int CompareTo(IntKey4 other)
			{
				int result = _arg1Type.CompareTo(other._arg1Type);

				if (result != 0)
					return result;

				result = _arg2Type.CompareTo(other._arg2Type);

				if (result != 0)
					return result;

				result = _arg3Type.CompareTo(other._arg3Type);

				if (result != 0)
					return result;

				return _arg4Type.CompareTo(other._arg4Type);
			}
		}

		#endregion

		#region Int64 keys

		private MethodInvoker MatchLongKey(object[] args)
		{
			MethodInvoker target = null;

			switch (_argumentCount)
			{
				case 1:
					LongKey1 key1 = new LongKey1(args);
					(_matches as SortedList<LongKey1, MethodInvoker>).TryGetValue(key1, out target);
					break;
				case 2:
					LongKey2 key2 = new LongKey2(args);
					(_matches as SortedList<LongKey2, MethodInvoker>).TryGetValue(key2, out target);
					break;
				case 3:
					LongKey3 key3 = new LongKey3(args);
					(_matches as SortedList<LongKey3, MethodInvoker>).TryGetValue(key3, out target);
					break;
				default:
					LongKey4 key4 = new LongKey4(args);
					(_matches as SortedList<LongKey4, MethodInvoker>).TryGetValue(key4, out target);
					break;
			}

			return target;
		}

		private void AddLongKey_InPlace(Type[] args, MethodInvoker target)
		{
			switch (_argumentCount)
			{
				case 1:
					LongKey1 key1 = new LongKey1(args);
					AddKey_InPlace(key1, target);
					break;
				case 2:
					LongKey2 key2 = new LongKey2(args);
					AddKey_InPlace(key2, target);
					break;
				case 3:
					LongKey3 key3 = new LongKey3(args);
					AddKey_InPlace(key3, target);
					break;
				default:
					LongKey4 key4 = new LongKey4(args);
					AddKey_InPlace(key4, target);
					break;
			}
		}

		private void AddLongKey(object[] args, MethodInvoker target)
		{
			switch (_argumentCount)
			{
				case 1:
					LongKey1 key1 = new LongKey1(args);
					AddKey(key1, target);
					break;
				case 2:
					LongKey2 key2 = new LongKey2(args);
					AddKey(key2, target);
					break;
				case 3:
					LongKey3 key3 = new LongKey3(args);
					AddKey(key3, target);
					break;
				default:
					LongKey4 key4 = new LongKey4(args);
					AddKey(key4, target);
					break;
			}
		}

		private struct LongKey1 : IComparable<LongKey1>
		{
			private long _arg1Type;

			public LongKey1(object[] args)
			{
				_arg1Type = args[0].GetType().TypeHandle.Value.ToInt64();
			}

			public LongKey1(Type[] args)
			{
				_arg1Type = args[0].TypeHandle.Value.ToInt64();
			}

			public int CompareTo(LongKey1 other)
			{
				return _arg1Type.CompareTo(other._arg1Type);
			}
		}

		private struct LongKey2 : IComparable<LongKey2>
		{
			private long _arg1Type;
			private long _arg2Type;

			public LongKey2(object[] args)
			{
				_arg1Type = args[0].GetType().TypeHandle.Value.ToInt64();
				_arg2Type = args[1].GetType().TypeHandle.Value.ToInt64();
			}

			public LongKey2(Type[] args)
			{
				_arg1Type = args[0].TypeHandle.Value.ToInt64();
				_arg2Type = args[1].TypeHandle.Value.ToInt64();
			}

			public int CompareTo(LongKey2 other)
			{
				int result = _arg1Type.CompareTo(other._arg1Type);

				if (result != 0)
					return result;

				return _arg2Type.CompareTo(other._arg2Type);
			}
		}

		private struct LongKey3 : IComparable<LongKey3>
		{
			private long _arg1Type;
			private long _arg2Type;
			private long _arg3Type;

			public LongKey3(object[] args)
			{
				_arg1Type = args[0].GetType().TypeHandle.Value.ToInt64();
				_arg2Type = args[1].GetType().TypeHandle.Value.ToInt64();
				_arg3Type = args[2].GetType().TypeHandle.Value.ToInt64();
			}

			public LongKey3(Type[] args)
			{
				_arg1Type = args[0].TypeHandle.Value.ToInt64();
				_arg2Type = args[1].TypeHandle.Value.ToInt64();
				_arg3Type = args[2].TypeHandle.Value.ToInt64();
			}

			public int CompareTo(LongKey3 other)
			{
				int result = _arg1Type.CompareTo(other._arg1Type);

				if (result != 0)
					return result;

				result = _arg2Type.CompareTo(other._arg2Type);

				if (result != 0)
					return result;

				return _arg3Type.CompareTo(other._arg3Type);
			}
		}

		private struct LongKey4 : IComparable<LongKey4>
		{
			private long _arg1Type;
			private long _arg2Type;
			private long _arg3Type;
			private long _arg4Type;

			public LongKey4(object[] args)
			{
				_arg1Type = args[0].GetType().TypeHandle.Value.ToInt64();
				_arg2Type = args[1].GetType().TypeHandle.Value.ToInt64();
				_arg3Type = args[2].GetType().TypeHandle.Value.ToInt64();
				_arg4Type = args[3].GetType().TypeHandle.Value.ToInt64();
			}

			public LongKey4(Type[] args)
			{
				_arg1Type = args[0].TypeHandle.Value.ToInt64();
				_arg2Type = args[1].TypeHandle.Value.ToInt64();
				_arg3Type = args[2].TypeHandle.Value.ToInt64();
				_arg4Type = args[3].TypeHandle.Value.ToInt64();
			}

			public int CompareTo(LongKey4 other)
			{
				int result = _arg1Type.CompareTo(other._arg1Type);

				if (result != 0)
					return result;

				result = _arg2Type.CompareTo(other._arg2Type);

				if (result != 0)
					return result;

				result = _arg3Type.CompareTo(other._arg3Type);

				if (result != 0)
					return result;

				return _arg4Type.CompareTo(other._arg4Type);
			}
		}

		#endregion
	}
}
