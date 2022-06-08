using Grpc.Core;
using IdeaStatiCa.PluginRunner.Utils;
using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.HealthCheck
{
	public class HealthCheckEndpoint : IHealthCheckEndpoint
	{
		public event Action? Pinged;

		private readonly IClientStreamWriter<Protos.CheckReq> _writer;
		private readonly CancellationToken _cancellationToken;

		public HealthCheckEndpoint(Protos.HealthCheck.HealthCheckClient client, CancellationToken cancellationToken)
		{
			_cancellationToken = cancellationToken;

			AsyncDuplexStreamingCall<Protos.CheckReq, Protos.CheckResp> watch
				= client.Watch(cancellationToken: cancellationToken);

			_writer = watch.RequestStream;
			Subscribe(watch.ResponseStream);
		}

		public Task Ping()
		{
			return _writer.WriteAsync(new Protos.CheckReq(), _cancellationToken);
		}

		private void Subscribe(IAsyncStreamReader<Protos.CheckResp> reader)
		{
			reader
				.AsObservable(_cancellationToken)
				.Subscribe(x => Pinged?.Invoke());
		}
	}
}