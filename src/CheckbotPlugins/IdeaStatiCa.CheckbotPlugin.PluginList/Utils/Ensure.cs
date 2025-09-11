using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Utils
{
	[DebuggerStepThrough]
	internal static class Ensure
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotNull<T>(T value, string name)
		{
			if (value is null)
			{
				Debug.Fail($"Value must not be null. Parameter '{name}'");
				throw new ArgumentNullException(name);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotEmpty(string value, string name)
		{
			if (string.IsNullOrEmpty(value))
			{
				Debug.Fail($"Value must not be null or empty. Parameter '{name}'");
				throw new ArgumentNullException(name);
			}
		}
	}
}