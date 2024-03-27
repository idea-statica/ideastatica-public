using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Utils
{
	[DebuggerStepThrough]
	internal static class Ensure
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotNull<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "")
		{
			if (value is null)
			{
				Debug.Fail($"Value must not be null. Parameter '{name}'");
				throw new ArgumentNullException(name);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotEmpty(string value, [CallerArgumentExpression(nameof(value))] string name = "")
		{
			if (string.IsNullOrEmpty(value))
			{
				Debug.Fail($"Value must not be null or empty. Parameter '{name}'");
				throw new ArgumentNullException(name);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Equals<T>(T val1, T val2, [CallerArgumentExpression(nameof(val1))] string name1 = "", [CallerArgumentExpression(nameof(val2))] string name2 = "")
		{
			if (!object.Equals(val1, val2))
			{
				Debug.Fail($"Values must be equal. Parameters '{name1}', '{name2}'");
				throw new ArgumentException(string.Format("Values must be equal: '{0}', '{1}'", name1, name2));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotEmpty<T>(IReadOnlyCollection<T> value, [CallerArgumentExpression(nameof(value))] string name = "")
		{
			NotNull(value, name);

			if (value.Count == 0)
			{
				Debug.Fail($"Collection must not be empty. Parameter '{name}'");
				throw new ArgumentException("Collection must not be empty.", name);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotNegative(int value, [CallerArgumentExpression(nameof(value))] string name = "")
		{
			if (value < 0)
			{
				Debug.Fail($"Value must not be negative. Parameter '{name}'");
				throw new ArgumentException("Value must not be negative.", name);
			}
		}
	}
}