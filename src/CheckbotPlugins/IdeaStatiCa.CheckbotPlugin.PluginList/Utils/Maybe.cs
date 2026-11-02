namespace IdeaStatiCa.CheckbotPlugin.PluginList.Utils
{
	internal abstract class Maybe<T>
	{
		internal sealed class None : Maybe<T>
		{
			public override Maybe<R> Bind<R>(Func<T, Maybe<R>> f)
			{
				return new Maybe<R>.None();
			}

			public override void Eval(Action<T> f)
			{ }

			public override T GetOrElse(T fallback)
			{
				return fallback;
			}

			public override T GetOrElse(Func<T> fallbackSource)
			{
				return fallbackSource();
			}

			public override T GetOrThrow(Exception exception)
			{
				throw exception;
			}

			public override Maybe<R> Map<R>(Func<T, R> f)
			{
				return new Maybe<R>.None();
			}
		}

		internal sealed class Some : Maybe<T>
		{
			private readonly T _value;

			public Some(T value)
			{
				_value = value;
			}

			internal T Get()
			{
				return _value;
			}

			public override Maybe<R> Bind<R>(Func<T, Maybe<R>> f)
			{
				return f(_value);
			}

			public override void Eval(Action<T> f)
			{
				f(_value);
			}

			public override T GetOrElse(T fallback)
			{
				return _value;
			}

			public override T GetOrElse(Func<T> fallbackSource)
			{
				return _value;
			}

			public override T GetOrThrow(Exception exception)
			{
				return _value;
			}

			public override Maybe<R> Map<R>(Func<T, R> f)
			{
				return new Maybe<R>.Some(f(_value));
			}
		}

		public static Maybe<T> Empty()
		{
			return new None();
		}

		public abstract Maybe<R> Map<R>(Func<T, R> f);

		public abstract Maybe<R> Bind<R>(Func<T, Maybe<R>> f);

		public abstract void Eval(Action<T> f);

		public abstract T GetOrElse(T fallback);

		public abstract T GetOrElse(Func<T> fallbackSource);

		public abstract T GetOrThrow(Exception exception);
	}
}