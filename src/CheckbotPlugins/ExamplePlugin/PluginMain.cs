using IdeaStatiCa.CheckbotPlugin;
using IdeaStatiCa.CheckbotPlugin.Models;
using IdeaStatiCa.CheckbotPlugin.Services;
using System.Diagnostics;

namespace ExamplePlugin
{
	public class PluginMain : IPlugin
	{
		public PluginInfo PluginInfo
			=> new("Example Plugin", "Tis a test plugin", "Me", "0.0.1");

		private readonly ConsoleManager _consoleManager;

		public PluginMain()
		{
			Debugger.Launch();

			_consoleManager = new ConsoleManager(true);
		}

		public void Entrypoint(IServiceProvider serviceProvider)
		{
			IEventService eventService = serviceProvider.GetService<IEventService>();
			IProjectService projectService = serviceProvider.GetService<IProjectService>();
			IApplicationService applicationService = serviceProvider.GetService<IApplicationService>();


			var settings = applicationService.GetAllSettings().GetAwaiter().GetResult();
			foreach (var set in settings)
			{
				Console.WriteLine($"{set.Name} = {set.Value}");
			}

			new Plugin(projectService, eventService).Run();
		}
	}
}