using AutoMapper;
using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.CheckbotPlugin.Services;

using Models = IdeaStatiCa.CheckbotPlugin.Models;

using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Services
{
	public class ApplicationService : IApplicationService
	{
		private static readonly Mapper _mapper = Mapping.GetMapper();

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
				.Select(Map)
				.ToList();
		}

		public async Task<string> GetSettings(string key)
		{
			Ensure.NotEmpty(key);

			Protos.GetSettingsReq reg = new();
			Protos.GetSettingsResp resp = await _client.GetSettingsAsync(reg);

			Models.SettingsValue settingsValue = Map(resp.Value);

			return settingsValue.Value;
		}

		private static Models.SettingsValue Map(Protos.SettingsValue settingsValue) => _mapper.Map<Models.SettingsValue>(settingsValue);
	}
}