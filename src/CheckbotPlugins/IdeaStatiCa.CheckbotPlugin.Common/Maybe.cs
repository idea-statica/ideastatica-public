using System;

namespace IdeaStatiCa.CheckbotPlugin.Common
{
	public abstract class Maybe<T>
	{
		internal sealed class None : Maybe<T>
		{
			public override Maybe<R> Bind<R>(Func<T, Maybe<R>> f)
				=> new Maybe<R>.None();

			public override void Eval(Action<T> f)
			{ }

			public override T GetOrElse(T fallback)
				=> fallback;

			public override T GetOrElse(Func<T> fallbackSource)
				=> fallbackSource();

			public override T GetOrThrow(Exception exception)
				=> throw exception;

			public override Maybe<R> Map<R>(Func<T, R> f)
				=> new Maybe<R>.None();
		}

		internal sealed class Some : Maybe<T>
		{
			private readonly T _value;

			public Some(T value)
			{
				_value = value;
			}

			internal T Get()
				=> _value;

			public override Maybe<R> Bind<R>(Func<T, Maybe<R>> f)
				=> f(_value);

			public override void Eval(Action<T> f)
				=> f(_value);

			public override T GetOrElse(T fallback)
				=> _value;

			public override T GetOrElse(Func<T> fallbackSource)
				=> _value;

			public override T GetOrThrow(Exception exception)
				=> _value;

			public override Maybe<R> Map<R>(Func<T, R> f)
				=> new Maybe<R>.Some(f(_value));
		}

		public static Maybe<T> Empty()
			=> new None();

		public abstract Maybe<R> Map<R>(Func<T, R> f);

		public abstract Maybe<R> Bind<R>(Func<T, Maybe<R>> f);

		public abstract void Eval(Action<T> f);

		public abstract T GetOrElse(T fallback);

		public abstract T GetOrElse(Func<T> fallbackSource);

		public abstract T GetOrThrow(Exception exception);
	}
}