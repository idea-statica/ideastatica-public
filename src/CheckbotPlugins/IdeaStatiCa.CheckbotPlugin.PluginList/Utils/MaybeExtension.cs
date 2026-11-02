namespace IdeaStatiCa.CheckbotPlugin.PluginList.Utils
{
	internal static class MaybeExtension
	{
		public static T? GetOrDefault<T>(this Maybe<T> value)
		{
			if (value is Maybe<T>.Some some)
			{
				return some.Get();
			}

			return default;
		}

		public static Maybe<T> ToMaybe<T>(this T? value)
		{
			if (value is null)
			{
				return new Maybe<T>.None();
			}
			return new Maybe<T>.Some(value);
		}

		public static Maybe<T> ToMaybe<T>(this T? value)
			where T : struct
		{
			if (value.HasValue)
			{
				return new Maybe<T>.Some(value.Value);
			}

			return new Maybe<T>.None();
		}
	}
}