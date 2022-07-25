using Grpc.Core;
using System.Collections.Immutable;

namespace IdeaStatiCa.PluginRunner.Utils
{
	internal static class AsyncStreamReaderExtension
	{
		public static IObservable<T> AsObservable<T>(this IAsyncStreamReader<T> streamReader, CancellationToken cancellationToken = default)
		{
			ObserableAsyncStreamReader<T> obserable = new(streamReader);
			obserable.Run(cancellationToken).ConfigureAwait(false);
			return obserable;
		}

		private sealed class ObserableAsyncStreamReader<T> : IObservable<T>
		{
			private ImmutableList<Subscription> _subscriptions = ImmutableList<Subscription>.Empty;
			private readonly IAsyncStreamReader<T> _streamReader;

			public ObserableAsyncStreamReader(IAsyncStreamReader<T> streamReader)
			{
				_streamReader = streamReader;
			}

			public IDisposable Subscribe(IObserver<T> observer)
			{
				Subscription subscription = new(this, observer);
				_subscriptions = _subscriptions.Add(subscription);
				return subscription;
			}

			internal void Unsubscribe(Subscription subscription)
			{
				_subscriptions = _subscriptions.Remove(subscription);
			}

			public async Task Run(CancellationToken cancellationToken)
			{
				bool onError = false;

				try
				{
					while (await _streamReader.MoveNext(cancellationToken))
					{
						_subscriptions.ForEach(x => x.Observer.OnNext(_streamReader.Current));
					}
				}
				catch (Exception ex)
				{
					onError = true;
					_subscriptions.ForEach(x => x.Observer.OnError(ex));
				}

				if (!onError)
				{
					_subscriptions.ForEach(x => x.Observer.OnCompleted());
				}
			}

			internal class Subscription : IDisposable
			{
				internal IObserver<T> Observer { get; }

				private bool _disposed;
				private readonly ObserableAsyncStreamReader<T> _obserable;

				public Subscription(ObserableAsyncStreamReader<T> obserable, IObserver<T> observer)
				{
					_obserable = obserable;
					Observer = observer;
				}

				public void Dispose()
				{
					if (_disposed)
					{
						throw new ObjectDisposedException(nameof(Subscription));
					}

					_obserable.Unsubscribe(this);
					_disposed = true;
				}
			}
		}
	}
}