#if !NET6_0_OR_GREATER

using System;
using System.Collections.Generic;

namespace IdeaStatiCa.CheckbotPlugin.Common
{
	public static class Ensure
	{
		public static void NotNull(object value, string argName)
		{
			if (value is null)
			{
				throw new ArgumentNullException(argName);
			}
		}

		public static void NotEmpty(string value, string argName)
		{
			NotNull(value, argName);

			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("String must not be empty.", argName);
			}
		}

		public static void NotEmpty<T>(ICollection<T> value, string argName = "")
		{
			if (value.Count == 0)
			{
				throw new ArgumentException("Collection must not be empty.", argName);
			}
		}
	}
}

#endif