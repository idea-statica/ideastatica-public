using IdeaStatiCa.CheckbotPlugin.Services;
using IdeaStatiCa.PluginRunner.Utils;

using Models = IdeaStatiCa.CheckbotPlugin.Models;

using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Services
{
	public class ApplicationService : IApplicationService
	{
		private readonly Protos.ApplicationService.ApplicationServiceClient _client;

		public ApplicationService(Protos.ApplicationService.ApplicationServiceClient client)
		{
			_client = client;
		}

		public async Task<IReadOnlyCollection<Models.SettingsValue>> GetAllSettings()
		{
			Protos.GetAllSettingsReq reg = new();
			Protos.GetAllSettingsResp resp = await _client.GetAllSettingsAsync(reg);

			return resp.Values
				.Select(x => new Models.SettingsValue(x.Name, x.Value))
				.ToList();
		}

		public async Task<string> GetSettings(string key)
		{
			Ensure.ArgNotEmpty(key);

			Protos.GetSettingsReq reg = new();
			Protos.GetSettingsResp resp = await _client.GetSettingsAsync(reg);

			return resp.Value.Value;
		}
	}
}