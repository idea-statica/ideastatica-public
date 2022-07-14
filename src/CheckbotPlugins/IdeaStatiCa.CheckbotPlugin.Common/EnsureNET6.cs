#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace IdeaStatiCa.CheckbotPlugin.Common
{
	public static class Ensure
	{
		public static void NotNull([NotNull] object? value, [CallerArgumentExpression("value")] string argName = "")
		{
			if (value is null)
			{
				throw new ArgumentNullException(argName);
			}
		}

		public static void NotEmpty([NotNull] string? value, [CallerArgumentExpression("value")] string argName = "")
		{
			NotNull(value, argName);

			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("String must not be empty.", argName);
			}
		}

		public static void NotEmpty<T>([NotNull] ICollection<T> value, [CallerArgumentExpression("value")] string argName = "")
		{
			if (value.Count == 0)
			{
				throw new ArgumentException("Collection must not be empty.", argName);
			}
		}
	}
}

#endif