namespace IdeaStatiCa.PluginRunner.HealthCheck
{
	public interface IHealthCheckEndpoint
	{
		event Action Pinged;

		Task Ping(CancellationToken cancellationToken);
	}
}