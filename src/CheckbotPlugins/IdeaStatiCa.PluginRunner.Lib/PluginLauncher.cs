using IdeaStatiCa.CheckbotPlugin;
using IdeaStatiCa.CheckbotPlugin.Models;
using System.Reflection;
using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner
{
	public struct PluginLaunchRequest
	{
		public IPlugin Plugin { get; }

		public PluginLaunchRequest(IPlugin plugin)
		{
			Plugin = plugin;
		}

		public PluginLaunchRequest(string path, string? className)
		{
			Assembly assembly = Assembly.LoadFrom(path);
			Type entryPointClass = PluginEntryPoint.GetEntryPointClass(assembly, className);
			Plugin = PluginEntryPoint.GetInstance(entryPointClass);
		}
	}

	public struct PluginLaunchResponse
	{
		public string ApiVersion { get; }

		public string IdeaVersion { get; }

		public PluginLaunchResponse(string apiVersion, string ideaVersion)
		{
			ApiVersion = apiVersion;
			IdeaVersion = ideaVersion;
		}
	}

	public class PluginLauncher
	{
		private readonly Protos.PluginService.PluginServiceClient _client;

		public PluginLauncher(Protos.PluginService.PluginServiceClient client)
		{
			_client = client;
		}

		public async Task<PluginLaunchResponse> Launch(IServiceProvider serviceProvider, PluginLaunchRequest request)
		{
			PluginInfo info = request.Plugin.PluginInfo;

			Protos.HelloResp helloResp = await _client.HelloAsync(new Protos.HelloReq()
			{
				ApiVersion = ApiVersion.Version,
				Author = info.Author,
				Description = info.Description,
				DisplayName = info.DisplayName,
				Version = info.Version
			});

			if (helloResp.Status != Protos.HelloStatus.Ok)
			{
				throw new Exception();
			}

			return new PluginLaunchResponse(helloResp.ApiVersion, helloResp.IdeaVersion);
		}

		public async Task Ready()
		{
			await _client.ReadyAsync(new Protos.ReadyReq());
		}
	}
}