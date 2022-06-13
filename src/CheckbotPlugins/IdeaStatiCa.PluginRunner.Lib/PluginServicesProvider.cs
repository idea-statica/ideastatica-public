using IdeaStatiCa.CheckbotPlugin.Services;
using System;

namespace IdeaStatiCa.PluginRunner
{
	internal class PluginServicesProvider : IServiceProvider
	{
		private readonly IApplicationService _applicationService;
		private readonly IEventService _eventService;
		private readonly IKeyValueStorage _keyValueStorage;
		private readonly IPluginService _pluginService;
		private readonly IProjectService _projectService;

		public PluginServicesProvider(
			IApplicationService applicationService,
			IEventService eventService,
			IKeyValueStorage keyValueStorage,
			IPluginService pluginService,
			IProjectService projectService)
		{
			_applicationService = applicationService;
			_eventService = eventService;
			_keyValueStorage = keyValueStorage;
			_pluginService = pluginService;
			_projectService = projectService;
		}

		public object? GetService(Type serviceType)
		{
			if (serviceType == typeof(IApplicationService))
			{
				return _applicationService;
			}
			else if (serviceType == typeof(IEventService))
			{
				return _eventService;
			}
			else if (serviceType == typeof(IKeyValueStorage))
			{
				return _keyValueStorage;
			}
			else if (serviceType == typeof(IPluginService))
			{
				return _pluginService;
			}
			else if (serviceType == typeof(IProjectService))
			{
				return _projectService;
			}

			return null;
		}
	}
}