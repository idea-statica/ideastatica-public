using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace IdeaStatiCa.PluginRunner.Utils
{
	internal static class Ensure
	{
		public static void ArgNotEmpty([NotNull] string? value, [CallerArgumentExpression("value")] string argName = "")
		{
			ArgNotNull(value, argName);

			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("String must not be empty.", argName);
			}
		}

		public static void ArgNotNull([NotNull] object? value, [CallerArgumentExpression("value")] string argName = "")
		{
			if (value is null)
			{
				throw new ArgumentNullException(argName);
			}
		}
	}
}