using IdeaStatiCa.CheckbotPlugin.Services;
using System;

namespace IdeaStatiCa.PluginRunner
{
	internal class PluginServicesProvider : IServiceProvider
	{
		private readonly IApplicationService _applicationService;

		public PluginServicesProvider(IApplicationService applicationService)
		{
			_applicationService = applicationService;
		}

		public object? GetService(Type serviceType)
		{
			if (serviceType == typeof(IApplicationService))
			{
				return _applicationService;
			}

			return null;
		}
	}
}