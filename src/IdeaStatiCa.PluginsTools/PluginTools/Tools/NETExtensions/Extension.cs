using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

#if !SILVERLIGHT
using CI.Geometry2D;
using CI.Geometry3D;
#endif

namespace CI
{
	/// <summary>
	/// The extension methods.
	/// </summary>
	public static class Extension
	{
		#region IList<T> extension methods

#if !SILVERLIGHT

		/// <summary>
		/// Adds the elements of the specified collection to the end of the destination list.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="destination">The sequence to add elements from collection.</param>
		/// <param name="collection">The collection whose elements should be added to the end of the destination list.
		/// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public static void AddRange<T>(this IList<T> destination, IEnumerable<T> collection)
		{
			var list = destination as List<T>;
			if (list != null)
			{
				list.AddRange(collection);
			}
			else
			{
				foreach (var i in collection)
				{
					destination.Add(i);
				}
			}
		}

		/// <summary>
		/// The extension for IList generic collection with Clonables types.
		/// Creates a new collection that is a copy of the listToClone instance.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="listToClone">The collection to Cloning.</param>
		/// <returns>A new collection that is a copy of the listToClone instance.</returns>
		public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}
#endif

		/// <summary>
		/// Returns distinct elements from a sequence. From two equal values is second removed.
		/// </summary>
		/// <param name="source">The sequence to remove duplicate elements from.</param>
		/// <param name="tolerance">The tolerance level for comparison.</param>
		/// <returns>An new instance of IList that contains distinct elements from the source sequence.</returns>
		public static IList<double> Distinct(this IEnumerable<double> source, double tolerance)
		{
			var list = source.ToList();
			list.Distinct(tolerance);
			return list;
		}

		/// <summary>
		/// Distincts elements from a sequence. From two equal values is second removed.
		/// </summary>
		/// <param name="source">The sequence to remove duplicate elements from.</param>
		/// <param name="tolerance">The tolerance level for comparison.</param>
		/// <param name="sorted">If sorted, only neighbour nubers are compared.</param>
		public static void Distinct(this List<double> source, double tolerance, bool sorted = true)
		{
			if (sorted)
			{
				DistinctSorted(source, tolerance);
			}
			else
			{
				DistinctUnsorted(source, tolerance);
			}
		}

		/// <summary>
		/// Sorts items in this list using comparison. The items with equal key don't change their position.
		/// Do not use, if is not necessary to keep a position of items with equal key.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="list">The collection to sorting.</param>
		/// <param name="comparison">The <c>System.Comparison&lt;T&gt;</c> to use when comparing elements.</param>
		/// <exception cref="System.ArgumentNullException">Comparison is null.</exception>
		public static void BubbleSort<T>(this IList<T> list, Comparison<T> comparison)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException("IList<T>.BubbleSort - comparison is null");
			}

			int count = list.Count;
			for (int i = count - 1; i > 0; --i)
			{
				bool sorted = true;
				for (int j = count - 2; j >= 0; --j)
				{
					int result = comparison.Invoke(list[j], list[j + 1]);
					if (result.Equals(1))
					{
						T temp = list[j];
						list[j] = list[j + 1];
						list[j + 1] = temp;
						sorted = false;
					}
				}

				if (sorted)
				{
					break;
				}
			}
		}

		/// <summary>
		/// Sorts items in this list using comparison. The items with equal key don't change their position.
		/// Do not use, if is not necessary to keep a position of items with equal key.
		/// </summary>
		/// <typeparam name="TSource">The type of elements in the list.</typeparam>
		/// <typeparam name="TKey">The type of value for tranformation of each element.</typeparam>
		/// <param name="list">The collection to sorting.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <exception cref="System.ArgumentNullException">Selector is null.</exception>
		public static void BubbleSort<TSource, TKey>(this IList<TSource> list, Func<TSource, TKey> selector)
			where TKey : IComparable<TKey>
		{
			if (selector == null)
			{
				throw new ArgumentNullException("IList<TSource>.BubbleSort - selector is null");
			}

			int count = list.Count;
			for (int i = count - 1; i > 0; --i)
			{
				bool sorted = true;
				for (int j = count - 2; j >= 0; --j)
				{
					var key1 = selector(list[j]);
					var key2 = selector(list[j + 1]);
					if (key1.CompareTo(key2) > 0)
					{
						var temp = list[j];
						list[j] = list[j + 1];
						list[j + 1] = temp;
						sorted = false;
					}
				}

				if (sorted)
				{
					break;
				}
			}
		}

		private static void DistinctSorted(this List<double> source, double tolerance)
		{
			for (var i = source.Count - 2; i >= 0; --i)
			{
				if (source[i].IsEqual(source[i + 1], tolerance))
				{
					source.RemoveAt(i + 1);
				}
			}
		}

		private static void DistinctUnsorted(this List<double> source, double tolerance)
		{
			var count = source.Count;
			var dec = new DoubleEqualityComparer { Tolerance = tolerance };
			var distincted = new List<double>(source.Count);
			for (var i = 0; i < count; ++i)
			{
				if (!distincted.Contains(source[i], dec))
					distincted.Add(source[i]);
			}

			source.Clear();
			source.AddRange(distincted);
		}

		/// <summary>
		/// Returns the index of first element of a sequence.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <param name="source">The System.Collections.Generic.List`1 to return the index of first element of.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <returns>The index of first element in the sequence that passes the test in the specified predicate function.</returns>
		public static int FirstIndexOf<TSource>(this IList<TSource> source, Predicate<TSource> predicate)
		{
			for (var i = 0; i < source.Count; ++i)
			{
				if (predicate(source[i]))
				{
					return i;
				}
			}

			return -1;
		}

		#endregion IList<T> extension methods

		#region IEnumerable<T> extension methods

		/// <summary>
		/// Determines whether a System.Collections.Generic.IEnumerable object (slave) is a subset of the specified collection (master).
		/// </summary>
		/// <typeparam name="T">The type of elements in the collections.</typeparam>
		/// <param name="slave">The slave collection.</param>
		/// <param name="master">The collection to compare to the slave System.Collections.Generic.IEnumerable object.</param>
		/// <returns>true if the slave object is a subset of master; otherwise, false.</returns>
		public static bool IsSubsetOf<T>(this IEnumerable<T> slave, IEnumerable<T> master)
		{
			return slave.Any() && !slave.Except(master).Any();
		}

		/// <summary>
		/// Returns the absolute maximum System.Double value or zero, if sequence is empty.
		/// </summary>
		/// <param name="source">A sequence of System.Double values to determine the absolute maximum value of.</param>
		/// <returns>The absolute maximum System.Double value or zero, if sequence is empty.</returns>
		public static double MaxAbsOrDefault(this IEnumerable<double> source)
		{
			double maxKey = 0;

			using (var enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					maxKey = enumerator.Current;
					while (enumerator.MoveNext())
					{
						var currentKey = enumerator.Current;
						if (Math.Abs(currentKey) > Math.Abs(maxKey))
						{
							maxKey = currentKey;
						}
					}
				}
			}

			return maxKey;
		}

		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the maximum System.Double value or zero, if sequence is empty.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>The maximum value in the sequence or zero, if sequence is empty.</returns>
		public static double MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			double maxKey = 0;

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					maxKey = selector(enumerator.Current);
					while (enumerator.MoveNext())
					{
						var currentKey = selector(enumerator.Current);
						if (currentKey > maxKey)
						{
							maxKey = currentKey;
						}
					}
				}
			}

			return maxKey;
		}

		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the minimum System.Double value or zero, if sequence is empty.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>The minimum value in the sequence or zero, if sequence is empty.</returns>
		public static double MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			double minKey = 0;

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					minKey = selector(enumerator.Current);
					while (enumerator.MoveNext())
					{
						var currentKey = selector(enumerator.Current);
						if (currentKey < minKey)
						{
							minKey = currentKey;
						}
					}
				}
			}

			return minKey;
		}

		/// <summary>
		/// Returns the maximum object in a generic sequence.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of value for tranformation of each element.</typeparam>
		/// <param name="source">A sequence of objects to determine the maximum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>The maximum object in the sequence.</returns>
		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
			where TKey : IComparable<TKey>
		{
			TSource maxObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new InvalidOperationException("Sequence is empty");
				}

				maxObj = enumerator.Current;
				var maxKey = selector(maxObj);
				while (enumerator.MoveNext())
				{
					TSource currentObj = enumerator.Current;
					TKey currentKey = selector(currentObj);
					if (currentKey.CompareTo(maxKey) > 0)
					{
						maxObj = currentObj;
						maxKey = currentKey;
					}
				}
			}

			return maxObj;
		}

		/// <summary>
		/// Returns the minimum object in a generic sequence.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of value for tranformation of each element.</typeparam>
		/// <param name="source">A sequence of objects to determine the minimum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>The minimum object in the sequence.</returns>
		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
			where TKey : IComparable<TKey>
		{
			TSource minObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new InvalidOperationException("Sequence is empty");
				}

				minObj = enumerator.Current;
				var minKey = selector(minObj);
				while (enumerator.MoveNext())
				{
					TSource currentObj = enumerator.Current;
					TKey currentKey = selector(currentObj);
					if (currentKey.CompareTo(minKey) < 0)
					{
						minObj = currentObj;
						minKey = currentKey;
					}
				}
			}

			return minObj;
		}

		/// <summary>
		/// Returns the maximum object in a generic sequence, or a default value if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of value for tranformation of each element.</typeparam>
		/// <param name="source">A sequence of objects to determine the maximum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>default(<typeparamref name="TSource" />) if <paramref name="source" /> is empty; otherwise, the maximum object in the sequence.</returns>
		public static TSource MaxByOrDefault<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
			where TKey : IComparable<TKey>
		{
			TSource maxObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					maxObj = enumerator.Current;
					var maxKey = selector(maxObj);
					while (enumerator.MoveNext())
					{
						TSource currentObj = enumerator.Current;
						TKey currentKey = selector(currentObj);
						if (currentKey.CompareTo(maxKey) > 0)
						{
							maxObj = currentObj;
							maxKey = currentKey;
						}
					}
				}
			}

			return maxObj;
		}

		/// <summary>
		/// Returns the maximum object in a generic sequence, or a default value if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects to determine the maximum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>default(<typeparamref name="TSource" />) if <paramref name="source" /> is empty; otherwise, the maximum object in the sequence.</returns>
		public static TSource MaxByOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			TSource maxObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					maxObj = enumerator.Current;
					var maxKey = selector(maxObj);
					while (enumerator.MoveNext())
					{
						TSource currentObj = enumerator.Current;
						var currentKey = selector(currentObj);
						if (currentKey.HasValue && (!maxKey.HasValue || currentKey > maxKey))
						{
							maxObj = currentObj;
							maxKey = currentKey;
						}
					}
				}
			}

			return maxObj;
		}

		/// <summary>
		/// Returns the maximum object in a generic sequence, or a default value if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects to determine the maximum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>default(<typeparamref name="TSource" />) if <paramref name="source" /> is empty; otherwise, the maximum object in the sequence.</returns>
		public static TSource MaxByOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			TSource maxObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					maxObj = enumerator.Current;
					var maxKey = selector(maxObj);
					while (enumerator.MoveNext())
					{
						TSource currentObj = enumerator.Current;
						var currentKey = selector(currentObj);
						if (currentKey > maxKey)
						{
							maxObj = currentObj;
							maxKey = currentKey;
						}
					}
				}
			}

			return maxObj;
		}

		/// <summary>
		/// Returns the maximum object in a generic sequence, or a zero value if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects to determine the maximum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>A zero value if <paramref name="source" /> is empty; otherwise, the maximum object in the sequence.</returns>
		public static TSource MaxByOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			TSource maxObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					maxObj = enumerator.Current;
					var maxKey = selector(maxObj);
					while (enumerator.MoveNext())
					{
						TSource currentObj = enumerator.Current;
						var currentKey = selector(currentObj);
						if (currentKey > maxKey)
						{
							maxObj = currentObj;
							maxKey = currentKey;
						}
					}
				}
			}

			return maxObj;
		}

		/// <summary>
		/// Returns the minimum object in a generic sequence, or a default value if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of value for tranformation of each element.</typeparam>
		/// <param name="source">A sequence of objects to determine the minimum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>default(<typeparamref name="TSource" />) if <paramref name="source" /> is empty; otherwise, the minimum object in the sequence.</returns>
		public static TSource MinByOrDefault<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
			where TKey : IComparable<TKey>
		{
			TSource minObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					minObj = enumerator.Current;
					var minKey = selector(minObj);
					while (enumerator.MoveNext())
					{
						TSource currentObj = enumerator.Current;
						TKey currentKey = selector(currentObj);
						if (currentKey.CompareTo(minKey) < 0)
						{
							minObj = currentObj;
							minKey = currentKey;
						}
					}
				}
			}

			return minObj;
		}

		/// <summary>
		/// Returns the minimum object in a generic sequence, or a default value if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects to determine the minimum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>default(<typeparamref name="TSource" />) if <paramref name="source" /> is empty; otherwise, the minimum object in the sequence.</returns>
		public static TSource MinByOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			TSource minObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					minObj = enumerator.Current;
					var minKey = selector(minObj);
					while (enumerator.MoveNext())
					{
						TSource currentObj = enumerator.Current;
						var currentKey = selector(currentObj);
						if (currentKey.HasValue && (!minKey.HasValue || currentKey < minKey))
						{
							minObj = currentObj;
							minKey = currentKey;
						}
					}
				}
			}

			return minObj;
		}

		/// <summary>
		/// Returns the minimum object in a generic sequence, or a default value if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects to determine the minimum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>default(<typeparamref name="TSource" />) if <paramref name="source" /> is empty; otherwise, the minimum object in the sequence.</returns>
		public static TSource MinByOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			TSource minObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					minObj = enumerator.Current;
					var minKey = selector(minObj);
					while (enumerator.MoveNext())
					{
						TSource currentObj = enumerator.Current;
						var currentKey = selector(currentObj);
						if (currentKey < minKey)
						{
							minObj = currentObj;
							minKey = currentKey;
						}
					}
				}
			}

			return minObj;
		}

		/// <summary>
		/// Returns the minimum object in a generic sequence, or a default value if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects to determine the minimum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>default(<typeparamref name="TSource" />) if <paramref name="source" /> is empty; otherwise, the minimum object in the sequence.</returns>
		public static TSource MinByOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			TSource minObj = default(TSource);

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					minObj = enumerator.Current;
					var minKey = selector(minObj);
					while (enumerator.MoveNext())
					{
						TSource currentObj = enumerator.Current;
						var currentKey = selector(currentObj);
						if (currentKey < minKey)
						{
							minObj = currentObj;
							minKey = currentKey;
						}
					}
				}
			}

			return minObj;
		}

		/// <summary>
		/// Returns the minimum and maximum object in a generic sequence, or a default values if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of value for tranformation of each element.</typeparam>
		/// <param name="source">A sequence of objects to determine the minimum and maximum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <param name="minObj">The minimal result of search.</param>
		/// <param name="maxObj">The maximal result of search.</param>
		public static void MinMaxByOrDefault<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, out TSource minObj, out TSource maxObj)
			where TKey : IComparable<TKey>
		{
			minObj = default(TSource);
			maxObj = minObj;

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					minObj = maxObj = enumerator.Current;
					var minKey = selector(minObj);
					var maxKey = minKey;
					while (enumerator.MoveNext())
					{
						var currentObj = enumerator.Current;
						var currentKey = selector(currentObj);

						if (currentKey.CompareTo(minKey) < 0)
						{
							minObj = currentObj;
							minKey = currentKey;
						}

						if (currentKey.CompareTo(maxKey) > 0)
						{
							maxObj = currentObj;
							maxKey = currentKey;
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns the minimum and maximum object in a generic sequence, or a default values if the sequence contains no elements..
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of value for tranformation of each element.</typeparam>
		/// <param name="source">A sequence of objects to determine the minimum and maximum object of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>An System.Collections.Generic.IEnumerable that has elements of type TSource that are obtained by min and max search criterium.</returns>
		public static IEnumerable<TSource> MinMaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
			where TKey : IComparable<TKey>
		{
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					TSource minObj, maxObj;
					minObj = maxObj = enumerator.Current;
					var minKey = selector(minObj);
					var maxKey = minKey;
					while (enumerator.MoveNext())
					{
						var currentObj = enumerator.Current;
						var currentKey = selector(currentObj);

						if (currentKey.CompareTo(minKey) < 0)
						{
							minObj = currentObj;
							minKey = currentKey;
						}

						if (currentKey.CompareTo(maxKey) > 0)
						{
							maxObj = currentObj;
							maxKey = currentKey;
						}
					}

					yield return minObj;
					yield return maxObj;
				}
			}
		}

		/// <summary>
		/// Invoke an <paramref name="action" /> on each element of sequence.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects to apply <paramref name="action" /></param>
		/// <param name="action">An anction to apply to each element.</param>
		public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
		{
			foreach (TSource current in source)
			{
				action(current);
			}
		}

		/// <summary>
		/// Rotates elements in collection.
		/// int[] numbers = { 1, 2, 3, 4, 5, 6, 7 };
		/// var circularNumbers = numbers.AsCircular();
		/// var firstFourNumbers = circularNumbers.Take(4); // 1 2 3 4
		/// var nextSevenNumbersfromfourth = circularNumbers
		/// .Skip(4).Take(7); // 4 5 6 7 1 2 3
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects to create circular iterator.</param>
		/// <returns>An System.Collections.Generic.IEnumerable that has elements of type TSource that has circular iterator.</returns>
		public static IEnumerable<TSource> AsCircular<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
				yield break; // be a gentleman

			IEnumerator<TSource> enumerator = source.GetEnumerator();

		iterateAllAndBackToStart:
			while (enumerator.MoveNext()) yield return enumerator.Current;
			enumerator.Reset();
			goto iterateAllAndBackToStart;
		}

		/// <summary>
		/// Determine, whether collection contains more than one item.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects.</param>
		/// <returns>true, if collection contains more than one item; false otherwise.</returns>
		public static bool HasMoreThanOneItem<TSource>(this IEnumerable<TSource> source)
		{
			return source.Take(2).Count() > 1;
		}

		/// <summary>
		/// Determine, whether collection contains specified number of items atleast.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of objects.</param>
		/// <param name="minCount">A minimal count of items in sequence.</param>
		/// <returns>true, if collection contains more minimal count; false otherwise.</returns>
		public static bool HasMoreThanItems<TSource>(this IEnumerable<TSource> source, int minCount)
		{
			return source.Skip(minCount).Any();
		}

		/// <summary>
		/// Returns distinct elements from a sequence by using a specified lambda expression to compare values.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The sequence to remove duplicate elements from.</param>
		/// <param name="lambda">The labmda expression to predicate duplicate elements.</param>
		/// <returns>An IEnumerable&lt;T&gt; that contains distinct elements from the source sequence.</returns>
		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> lambda)
		{
			return source.Distinct(new LambdaComparer<T>(lambda));
		}

		#endregion IEnumerable<T> extension methods

		#region ObservableCollection<T> extension methods

		/// <summary>
		/// JPJ - find method for observable collection
		/// </summary>
		/// <typeparam name="T">type of class</typeparam>
		/// <param name="collection">the observable collection</param>
		/// <param name="match">predicate to match</param>
		/// <returns>the object in the collection if found, otherwise returns null</returns>
		public static T Find<T>(this ObservableCollection<T> collection, Predicate<T> match)
		{
			T found = default(T);

			foreach (T item in collection)
			{
				if (match(item))
				{
					found = item;
					break;
				}
			}

			return found;
		}

		/// <summary>
		/// JPJ - AddRange method for observable collection
		/// </summary>
		/// <typeparam name="T">type of class</typeparam>
		/// <param name="collection">the observable collection</param>
		/// <param name="addCollection">collection to be added</param>
		public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> addCollection)
		{
			foreach (var item in addCollection)
			{
				collection.Add(item);
			}
		}

		#endregion ObservableCollection<T> extension methods

		#region Double extension methods

		/// <summary>
		/// Checks, if value is zero with specified tolerance.
		/// </summary>
		/// <param name="value">The value for check.</param>
		/// <param name="tolerance">The precision of check.</param>
		/// <returns>True, if value is zero, false otherwise.</returns>
		public static bool IsZero(this double value, double tolerance = 1e-9)
		{
			return Math.Abs(value) < tolerance;
		}

		/// <summary>
		/// Correction of double value acc. to limit
		/// </summary>
		/// <param name="value">value</param>
		/// <param name="limit">limit</param>
		/// <returns>double</returns>
		public static double Correct(this double value, double limit = 1e0)
		{
			if (limit.IsZero(1e-12))
			{
				return value;
			}

			double d = value / limit;
			//// dd = d > 0 ? Math.Floor(d) : Math.Ceiling(d);
			double dd = Math.Floor(d);
			if (d - dd >= 0.5)
			{
				dd += 1;
			}

			return dd * limit;
		}

		/// <summary>
		/// IsEqual - Determines whether leftValue and rightValue are equal.
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue and rightValue are equal. Return false otherwise</returns>
		public static bool IsEqual(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			if ((double.IsPositiveInfinity(leftValue) && double.IsPositiveInfinity(rightValue)) ||
				(double.IsNegativeInfinity(leftValue) && double.IsNegativeInfinity(rightValue)))
			{
				return true;
			}

			return Math.Abs(leftValue - rightValue) <= tolerance;
		}

		/// <summary>
		/// IsGreater - Determines whether the leftValue is greater than rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is greater than rightValue. Return false otherwise</returns>
		public static bool IsGreater(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			return (leftValue - rightValue) >= tolerance;
		}

		/// <summary>
		/// IsGreaterOrEqual - Determines whether the leftValue is greater or equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue is greater than or equal to rightValue. Return false otherwise</returns>
		public static bool IsGreaterOrEqual(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			if ((double.IsPositiveInfinity(leftValue) && double.IsPositiveInfinity(rightValue)) ||
				(double.IsNegativeInfinity(leftValue) && double.IsNegativeInfinity(rightValue)))
			{
				return true;
			}

			return (rightValue - leftValue) <= tolerance;
		}

		/// <summary>
		/// IsLesser - Determines whether leftValue is lesser than rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is lesser than rightValue. Return false otherwise</returns>
		public static bool IsLesser(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			return (rightValue - leftValue) >= tolerance;
		}

		/// <summary>
		/// IsLesserOrEqual - Determines whether the leftValue is lesser or equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is lesser than or equal to rightValue. Return false otherwise</returns>
		public static bool IsLesserOrEqual(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			if ((double.IsPositiveInfinity(leftValue) && double.IsPositiveInfinity(rightValue)) ||
				(double.IsNegativeInfinity(leftValue) && double.IsNegativeInfinity(rightValue)))
			{
				return true;
			}

			return (leftValue - rightValue) <= tolerance;
		}

		#endregion Double extension methods

		#region Float extension methods

		/// <summary>
		/// Checks, if value is zero with specified tolerance.
		/// </summary>
		/// <param name="value">The value for check.</param>
		/// <param name="tolerance">The precision of check.</param>
		/// <returns>True, if value is zero, false otherwise.</returns>
		public static bool IsZero(this float value, float tolerance = 1e-9f)
		{
			return Math.Abs(value) < tolerance;
		}

		/// <summary>
		/// Correction of float value acc. to limit
		/// </summary>
		/// <param name="value">value</param>
		/// <param name="limit">limit</param>
		/// <returns>float</returns>
		public static double Correct(this float value, float limit = 1e0f)
		{
			if (limit.IsZero(1e-12f))
			{
				return value;
			}

			double d = value / limit;
			//// dd = d > 0 ? Math.Floor(d) : Math.Ceiling(d);
			double dd = Math.Floor(d);
			if (d - dd >= 0.5)
			{
				dd += 1;
			}

			return dd * limit;
		}

		/// <summary>
		/// IsEqual - Determines whether leftValue and rightValue are equal.
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue and rightValue are equal. Return false otherwise</returns>
		public static bool IsEqual(this float leftValue, float rightValue, float tolerance = 1e-10f)
		{
			return Math.Abs(leftValue - rightValue) <= tolerance;
		}

		/// <summary>
		/// IsGreater - Determines whether the leftValue is greater than rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is greater than rightValue. Return false otherwise</returns>
		public static bool IsGreater(this float leftValue, float rightValue, float tolerance = 1e-10f)
		{
			return (leftValue - rightValue) >= tolerance;
		}

		/// <summary>
		/// IsGreaterOrEqual - Determines whether the leftValue is greater or equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue is greater than or equal to rightValue. Return false otherwise</returns>
		public static bool IsGreaterOrEqual(this float leftValue, float rightValue, double tolerance = 1e-10f)
		{
			return (rightValue - leftValue) <= tolerance;
		}

		/// <summary>
		/// IsLesser - Determines whether leftValue is lesser than rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is lesser than rightValue. Return false otherwise</returns>
		public static bool IsLesser(this float leftValue, float rightValue, float tolerance = 1e-10f)
		{
			return (rightValue - leftValue) >= tolerance;
		}

		/// <summary>
		/// IsLesserOrEqual - Determines whether the leftValue is lesser or equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is lesser than or equal to rightValue. Return false otherwise</returns>
		public static bool IsLesserOrEqual(this float leftValue, float rightValue, float tolerance = 1e-10f)
		{
			return (leftValue - rightValue) <= tolerance;
		}

		#endregion Float extension methods

		#region Point extension methods

#if !SILVERLIGHT

		/// <summary>
		/// IsEqualWithTolerance - Determines whether the leftValue is equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>True if equal</returns>
		public static bool IsEqualWithTolerance(this IdaComPoint2D leftValue, IdaComPoint2D rightValue, double tolerance = 1e-9)
		{
			return (Math.Abs(leftValue.X - rightValue.X) <= tolerance) && (Math.Abs(leftValue.Y - rightValue.Y) <= tolerance);
		}

		/// <summary>
		/// IsEqualWithTolerance - Determines whether the leftValue is equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>True if equal</returns>
		public static bool IsEqualWithTolerance(this Point leftValue, Point rightValue, double tolerance = 1e-9)
		{
			return (Math.Abs(leftValue.X - rightValue.X) <= tolerance) && (Math.Abs(leftValue.Y - rightValue.Y) <= tolerance);
		}

		/// <summary>
		/// IsEqualWithTolerance - Determines whether the leftValue is equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>True if equal</returns>
		public static bool IsEqualWithTolerance(this IPoint3D leftValue, IPoint3D rightValue, double tolerance = 1e-9)
		{
			return (Math.Abs(leftValue.X - rightValue.X) <= tolerance) && (Math.Abs(leftValue.Y - rightValue.Y) <= tolerance) && (Math.Abs(leftValue.Z - rightValue.Z) <= tolerance);
		}

		/// <summary>
		/// Returns true is X or Y or Z is not a number
		/// </summary>
		/// <param name="src">Source point</param>
		/// <returns>Returns true is X or Y or Z is not a number</returns>
		public static bool IsNaN(this IPoint3D src)
		{
			return (double.IsNaN(src.X) || double.IsNaN(src.Y) || double.IsNaN(src.Z));
		}

#endif

#if !SILVERLIGHT

		/// <summary>
		/// IsEqualWithToleranceRelativeY (for CompareRegion2DGeometryRelative) - Determines whether the leftValue is equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>True if equal</returns>
		public static bool IsEqualWithToleranceRelativeY(this IdaComPoint2D leftValue, IdaComPoint2D rightValue, double tolerance = 1e-9)
		{
			return (Math.Abs(leftValue.X - rightValue.X) <= tolerance) && (Math.Abs((Math.Abs(leftValue.Y) - Math.Abs(rightValue.Y))) <= tolerance);
		}

		/// <summary>
		/// IsEqualWithToleranceRelativeY (for CompareRegion2DGeometryRelative) - Determines whether the leftValue is equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>True if equal</returns>
		public static bool IsEqualWithToleranceRelativeY(this Point leftValue, Point rightValue, double tolerance = 1e-9)
		{
			return (Math.Abs(leftValue.X - rightValue.X) <= tolerance) && ((Math.Abs(leftValue.Y) - Math.Abs(rightValue.Y)) <= tolerance);
		}

#endif

		/// <summary>
		/// Distance - Determines the distance between two points
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <returns>Distance between two points</returns>
		public static double Distance(this Point leftValue, Point rightValue)
		{
			return Math.Sqrt(Math.Pow(leftValue.X - rightValue.X, 2.0) + Math.Pow(leftValue.Y - rightValue.Y, 2.0));
		}

		/// <summary>
		/// MiddlePoint
		/// </summary>
		/// <param name="leftValue"></param>
		/// <param name="rightValue"></param>
		/// <returns></returns>
		public static Point MiddlePoint(this Point leftValue, Point rightValue)
		{
			return new Point((leftValue.X + rightValue.X) / 2, (leftValue.Y + rightValue.Y) / 2);
		}

		#endregion Point extension methods

		#region Matrix extension

		/// <summary>
		/// Mirror - add mirror transformation into the matrix, axis = 'x' or 'y'
		/// </summary>
		/// <param name="value">The transformation matrix.</param>
		/// <param name="axis">mirror axis</param>
		/// <returns>Matrix</returns>
		public static Matrix Mirror(this Matrix value, char axis)
		{
			if (axis == 'y')
			{
				value.M11 = -1.0;
			}
			else
			{
				value.M22 = -1.0;
			}

			return value;
		}

		#endregion Matrix extension

#if !SILVERLIGHT
		#region Exception extensions

		public static string GetDescription(this Exception value, int level)
		{
			int innerExeptionCount = 0;
			Exception innerEx = value.InnerException;

			StringBuilder msg = new StringBuilder(String.Format("{0} \n {1} \n{2} \n", value.Message, value.StackTrace, value.Source));
			while (innerEx != null && innerExeptionCount < level)
			{
				msg.Append(String.Format("Inner exception\n {0} \n {1} \n{2} \n", innerEx.Message, innerEx.StackTrace, innerEx.Source));

				innerExeptionCount++;
				innerEx = innerEx.InnerException;
			}

			return msg.ToString();
		}

		#endregion Exception extensions
#endif
	}
}