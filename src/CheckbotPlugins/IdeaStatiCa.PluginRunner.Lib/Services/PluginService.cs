using IdeaStatiCa.CheckbotPlugin.Services;
using IdeaStatiCa.PluginRunner.Utils;
using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Services
{
	public class PluginService : IPluginService
	{
		private readonly Protos.PluginService.PluginServiceClient _client;

		public PluginService(Protos.PluginService.PluginServiceClient client)
		{
			_client = client;
		}

		public async Task NewVersion(string newVersion)
		{
			Ensure.ArgNotEmpty(newVersion);

			Protos.NewVersionReq req = new()
			{
				NewVersion = newVersion
			};

			await _client.NewVersionAsync(req);
		}

		public async Task ProcedureComplete()
		{
			await _client.ProcedureCompletedAsync(new Protos.ProcedureCompleteReq());
		}
	}
}