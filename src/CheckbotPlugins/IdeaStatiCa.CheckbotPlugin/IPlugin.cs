using IdeaStatiCa.CheckbotPlugin.Models;
using System;

namespace IdeaStatiCa.CheckbotPlugin
{
	/// <summary>
	///
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// Returns
		/// </summary>
		PluginInfo PluginInfo { get; }

		/// <summary>
		///
		/// </summary>
		/// <param name="serviceProvider"></param>
		void Entrypoint(IServiceProvider serviceProvider);
	}
}