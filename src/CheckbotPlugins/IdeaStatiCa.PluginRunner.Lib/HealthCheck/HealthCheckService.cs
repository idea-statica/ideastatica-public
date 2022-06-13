using System.Reactive.Linq;

namespace IdeaStatiCa.PluginRunner.HealthCheck
{
	public class HealthCheckService<T> where T : HealthState<T>, IEquatable<T>
	{
		public delegate void StateChangedHandler(T newState, T oldState);

		public event StateChangedHandler? StateChanged;

		private readonly IHealthCheckEndpoint _endpoint;
		private readonly IHealthChecker<T> _checker;

		public HealthCheckService(IHealthCheckEndpoint healthCheckEndpoint, IHealthChecker<T> healthChecker)
		{
			_endpoint = healthCheckEndpoint;
			_checker = healthChecker;
		}

		public void Run(CancellationToken cancellationToken = default)
		{
			RunObserver(GetTickFunc(), cancellationToken);
		}

		private void RunObserver(Action<bool> tick, CancellationToken cancellationToken)
		{
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

			Observable.Merge(GetPingObserable(), GetTimerObserable())
				.Subscribe(
				x => tick(x),
				   ex => cancellationTokenSource.Cancel(),
				   () => cancellationTokenSource.Cancel(),
				   cancellationTokenSource.Token);
		}

		private static IObservable<bool> GetTimerObserable()
		{
			return Observable.Interval(TimeSpan.FromSeconds(1))
							.Select(_ => false);
		}

		private IObservable<bool> GetPingObserable()
		{
			return Observable.FromEvent<Action, bool>(
							x => () => x(true),
							   x => _endpoint.Pinged += x,
							   x => _endpoint.Pinged -= x);
		}

		private Action<bool> GetTickFunc()
		{
			T state = _checker.NewState();
			return (bool gotHeartbeat) =>
			{
				T newState = _checker.Tick(state, DateTime.Now, gotHeartbeat);

				if (newState.PingNow)
				{
					_endpoint.Ping().ConfigureAwait(false);
				}

				if (!newState.Equals(state))
				{
					StateChanged?.Invoke(newState, state);
				}

				state = newState;
			};
		}
	}
}