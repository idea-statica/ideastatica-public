using IdeaStatiCa.CheckbotPlugin.Models;
using IdeaStatiCa.CheckbotPlugin.Services;
using System;

namespace IdeaStatiCa.CheckbotPlugin
{
	/// <summary>
	/// Checkbot plugin interface. Class implementing this interface must be public.
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// Returns information about the plugin.
		/// </summary>
		PluginInfo PluginInfo { get; }

		/// <summary>
		/// Entrypoint method, called when the plugin is started.
		/// This method should return as soon as possible, after that the plugin
		/// will start receiving events/requests from the Checkbot.
		/// </summary>
		/// <param name="serviceProvider">Provider for Checkbot plugin services
		/// (e.g. <see cref="IProjectService"/>, <see cref="IKeyValueStorage"/>, ...).</param>
		void Entrypoint(IServiceProvider serviceProvider);
	}
}