#if NET5_0_OR_GREATER
#nullable disable
#endif

using System;

namespace IdeaStatiCa.CheckbotPlugin.Common
{
	public static class Maybe
	{
		public static Maybe<T> From<T>(T value)
		{
			return new Maybe<T>(value);
		}
	}

	public class Maybe<T>
	{
		private readonly bool _hasValue;
		private readonly T _value;

		public Maybe()
		{
		}

		public Maybe(T value)
		{
			if (value != null)
			{
				_hasValue = true;
				_value = value;
			}
		}

		public Maybe<R> Map<R>(Func<T, R> f)
		{
			if (_hasValue)
			{
				return new Maybe<R>(f(_value));
			}
			else
			{
				return new Maybe<R>();
			}
		}

		public Maybe<R> Bind<R>(Func<T, Maybe<R>> f)
		{
			if (_hasValue)
			{
				return f(_value);
			}
			else
			{
				return new Maybe<R>();
			}
		}

		public void Eval(Action<T> f)
		{
			if (_hasValue)
			{
				f(_value);
			}
		}

		public T Get(T fallback)
		{
			if (_hasValue)
			{
				return _value;
			}

			return fallback;
		}
	}
}