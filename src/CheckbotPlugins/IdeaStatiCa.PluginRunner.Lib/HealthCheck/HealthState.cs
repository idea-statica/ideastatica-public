namespace IdeaStatiCa.PluginRunner.HealthCheck
{
	public abstract class HealthState<T> where T : HealthState<T>, IEquatable<T>
	{
		public bool IsDead { get; }

		public bool PingNow { get; }

		protected HealthState(bool isDead, bool pingNow)
		{
			IsDead = isDead;
			PingNow = pingNow;
		}

		public T WithIsDead()
			=> Create(true, PingNow);

		public T WithPingNow()
			=> Create(IsDead, true);

		protected abstract T Create(bool isDead, bool pingNow);
	}
}