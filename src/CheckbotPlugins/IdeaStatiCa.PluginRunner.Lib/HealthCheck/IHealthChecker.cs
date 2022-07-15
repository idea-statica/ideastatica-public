namespace IdeaStatiCa.PluginRunner.HealthCheck
{
	public interface IHealthChecker<T> where T : HealthState<T>, IEquatable<T>
	{
		T NewState();

		T Tick(T state, DateTime now, bool gotHeartbeat);
	}
}