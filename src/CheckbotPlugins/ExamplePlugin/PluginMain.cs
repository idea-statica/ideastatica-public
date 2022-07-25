using IdeaStatiCa.CheckbotPlugin;
using IdeaStatiCa.CheckbotPlugin.Models;
using IdeaStatiCa.CheckbotPlugin.Services;

namespace ExamplePlugin
{
	public class PluginMain : IPlugin
	{
		public PluginInfo PluginInfo
			=> new("Example Plugin", "Tis a test plugin", "Me", "0.0.1");

		private Plugin? _plugin;
		private readonly ConsoleManager _consoleManager;

		public PluginMain()
		{
			_consoleManager = new ConsoleManager(true);
		}

		public void Entrypoint(IServiceProvider serviceProvider)
		{
			IEventService eventService = serviceProvider.GetService<IEventService>();
			IProjectService projectService = serviceProvider.GetService<IProjectService>();

			_plugin = new Plugin(projectService, eventService);
		}
	}
}