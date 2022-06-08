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
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

			IObservable<bool> pingObserable = Observable.FromEvent<Action, bool>(
				x => () => x(true),
				   x => _endpoint.Pinged += x,
				   x => _endpoint.Pinged -= x);

			IObservable<bool> intervalObserable = Observable.Interval(TimeSpan.FromSeconds(1))
				.Select(_ => false);

			T state = _checker.NewState();

			Observable.Merge(pingObserable, intervalObserable)
				.Subscribe(
				x => Tick(x),
				   ex => cancellationTokenSource.Cancel(),
				   () => cancellationTokenSource.Cancel(),
				   cancellationTokenSource.Token);

			void Tick(bool gotHeartbeat)
			{
				T newState = _checker.Tick(state, DateTime.Now, gotHeartbeat);

				if (newState.PingNow)
				{
					_endpoint.Ping(cancellationToken).ConfigureAwait(false);
				}

				if (!newState.Equals(state))
				{
					StateChanged?.Invoke(newState, state);
				}

				state = newState;
			}
		}
	}
}