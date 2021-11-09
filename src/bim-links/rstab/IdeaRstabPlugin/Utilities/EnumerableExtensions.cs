using System;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Utilities
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<TResult> SelectPairs<TSource, TResult>(this IEnumerable<TSource> source,
			Func<TSource, TSource, TResult> selector)
		{
			if (source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (selector is null)
			{
				throw new ArgumentNullException(nameof(selector));
			}

			return SelectPairsInternal(source, selector);
		}

		private static IEnumerable<TResult> SelectPairsInternal<TSource, TResult>(this IEnumerable<TSource> source,
			Func<TSource, TSource, TResult> selector)
		{
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				TSource prev = default;

				for (int i = 0; enumerator.MoveNext(); i++)
				{
					TSource cur = enumerator.Current;

					if (i > 0)
					{
						yield return selector(prev, cur);
					}

					prev = cur;
				}
			}
		}
	}
}