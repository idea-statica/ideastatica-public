using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.CheckbotPlugin.Services;
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
			Ensure.NotEmpty(newVersion);

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