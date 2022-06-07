using IdeaStatiCa.CheckbotPlugin.Protos;
using IdeaStatiCa.CheckbotPlugin.Services;
using IdeaStatiCa.PluginRunner.Utils;

using Models = IdeaStatiCa.CheckbotPlugin.Models;

namespace IdeaStatiCa.PluginRunner.Services
{
	public class ApplicationServiceGrpc : IApplicationService
	{
		private readonly ApplicationService.ApplicationServiceClient _client;

		public ApplicationServiceGrpc(ApplicationService.ApplicationServiceClient client)
		{
			_client = client;
		}

		public async Task<IReadOnlyCollection<Models.SettingsValue>> GetAllSettings()
		{
			GetAllSettingsReq reg = new();
			GetAllSettingsResp resp = await _client.GetAllSettingsAsync(reg);

			return resp.Values
				.Select(x => new Models.SettingsValue(x.Name, x.Value))
				.ToList();
		}

		public async Task<string> GetSettings(string key)
		{
			Ensure.ArgNotEmpty(key);

			GetSettingsReq reg = new();
			GetSettingsResp resp = await _client.GetSettingsAsync(reg);

			return resp.Value.Value;
		}
	}
}