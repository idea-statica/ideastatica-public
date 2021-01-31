using System;
using System.Collections.Generic;
using System.Reflection;

namespace CI.Common.MultiMethods
{
	public class DispatcherCache
	{
		private static CopyOnWriteDictionary<RuntimeTypeHandle, DispatcherCache> typeCache =
			new CopyOnWriteDictionary<RuntimeTypeHandle, DispatcherCache>();

		private CopyOnWriteDictionary<RuntimeMethodHandle, Dispatcher> _methodCache =
			new CopyOnWriteDictionary<RuntimeMethodHandle, Dispatcher>();

		private Type _implementationType;

		private DispatcherCache(Type implementationType)
		{
			_implementationType = implementationType;
		}

		public static DispatcherCache ForType(Type type)
		{
			DispatcherCache cache = typeCache[type.TypeHandle];

			if (cache == null)
			{
				typeCache[type.TypeHandle] = cache = new DispatcherCache(type);
			}

			return cache;
		}

		public int Size
		{
			get { return _methodCache.Size; }
		}

		public static void Clear()
		{
			typeCache = new CopyOnWriteDictionary<RuntimeTypeHandle, DispatcherCache>();
		}

		public Dispatcher DispatcherFor(MethodInfo multiMethod)
		{
			Dispatcher dispatcher = _methodCache[multiMethod.MethodHandle];

			if (dispatcher == null)
			{
				_methodCache[multiMethod.MethodHandle] = dispatcher =
					Dispatcher.Create(_implementationType, multiMethod, IntPtr.Size == 4);
			}

			return dispatcher;
		}

		private class CopyOnWriteDictionary<TKey, TValue>
		{
			private volatile Dictionary<TKey, TValue> _current = new Dictionary<TKey, TValue>();

			public TValue this[TKey key]
			{
				get
				{
					TValue value;
					_current.TryGetValue(key, out value);
					return value;
				}
				set
				{
					Dictionary<TKey, TValue> copy = new Dictionary<TKey, TValue>(_current);
					copy[key] = value;
					_current = copy;
				}
			}

			public int Size
			{
				get { return _current.Count; }
			}
		}
	}
}
