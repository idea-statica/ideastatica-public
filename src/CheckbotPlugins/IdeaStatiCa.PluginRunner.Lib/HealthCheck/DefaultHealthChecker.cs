namespace IdeaStatiCa.PluginRunner.HealthCheck
{
	public class DefaultHealthState : HealthState<DefaultHealthState>, IEquatable<DefaultHealthState>
	{
		public static DefaultHealthState New => new(false, false, DateTime.UtcNow);

		public DateTime LastHeartBeat { get; }

		internal DefaultHealthState(bool isDead, bool pingNow, DateTime lastHeartBeat)
			: base(isDead, pingNow)
		{
			LastHeartBeat = lastHeartBeat;
		}

		public DefaultHealthState WithHeartbeat(DateTime now)
			=> new(IsDead, PingNow, now);

		protected override DefaultHealthState Create(bool isDead, bool pingNow)
			=> new(isDead, pingNow, LastHeartBeat);

		public bool Equals(DefaultHealthState? other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (other is null)
			{
				return false;
			}

			return IsDead == other.IsDead
				&& PingNow == other.PingNow
				&& LastHeartBeat == other.LastHeartBeat;
		}
	}

	public class DefaultHealthChecker : IHealthChecker<DefaultHealthState>
	{
		private readonly TimeSpan _period;
		private readonly TimeSpan _timeout;

		public DefaultHealthChecker(TimeSpan period, TimeSpan timeout)
		{
			_period = period;
			_timeout = timeout;
		}

		public DefaultHealthState NewState() => DefaultHealthState.New;

		public DefaultHealthState Tick(DefaultHealthState state, DateTime now, bool gotHeartbeat)
		{
			if (gotHeartbeat)
			{
				state = state.WithHeartbeat(now).WithPingNow();
			}

			if (now - state.LastHeartBeat > _period)
			{
				state = state.WithPingNow();
			}

			if (now - state.LastHeartBeat > _timeout)
			{
				state = state.WithIsDead();
			}

			return state;
		}
	}
}