using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Factories
{
	/// <summary>
	/// Storage of objects. It track objects' changes by their hash codes.
	/// </summary>
	/// <typeparam name="T">Type to store</typeparam>
	public class ObjectStorage<T>
	{
		private static readonly IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.factories");

		private readonly Dictionary<int, (int hashCode, T obj)> _objects = new Dictionary<int, (int, T)>();
		private readonly IEqualityComparer<T> _equalityComparer;

		/// <summary>
		/// Create an instance of ObjectStorage.
		/// </summary>
		/// <param name="comparer">Comparer </param>
		public ObjectStorage(IEqualityComparer<T> comparer)
		{
			_equalityComparer = comparer;
		}

		/// <summary>
		/// Create an instance with a default comparer.
		/// </summary>
		public ObjectStorage() : this(EqualityComparer<T>.Default)
		{
		}

		/// <summary>
		/// Removes all stored objects.
		/// </summary>
		public void Clear()
		{
			_objects.Clear();
		}

		/// <summary>
		/// Return number of objects.
		/// </summary>
		public int Count()
		{
			if (_objects != null)
			{
				return _objects.Count;
			}
			return 0;
		}

		/// <summary>
		/// Get or create a new object by given <paramref name="index"/>.
		/// If object does not exist it creates it by given <paramref name="factory"/> function.
		/// If a hash code of the stored object has changed it is recreated.
		/// </summary>
		/// <param name="index">Index</param>
		/// <param name="factory">Object factory</param>
		/// <returns>Stored or newly created object</returns>
		public T GetOrCreate(int index, Func<T> factory)
		{
			if (_objects.TryGetValue(index, out var pair))
			{
				if (pair.hashCode == _equalityComparer.GetHashCode(pair.obj))
				{
					_logger.LogDebug($"Instance of {nameof(T)} for id {index} exists, reusing");

					return pair.obj;
				}

				_logger.LogDebug($"Instance of {nameof(T)} for id {index} already exists but hash codes do not match");
			}

			_logger.LogDebug($"Creating new {nameof(T)} for id {index}");

			T obj = factory();

			if (obj != null)
			{
				_objects[index] = (_equalityComparer.GetHashCode(obj), obj);
			}

			return obj;
		}
	}
}