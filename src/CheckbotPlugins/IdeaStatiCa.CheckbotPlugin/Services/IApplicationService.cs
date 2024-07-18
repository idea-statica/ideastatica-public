using IdeaStatiCa.CheckbotPlugin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	/// <summary>
	/// Provides application settings (units, language, etc.).
	/// </summary>
	public interface IApplicationService
	{
		/// <summary>
		/// Returns the application settings.
		/// </summary>
		/// <param name="key">Settings key</param>
		/// <returns>Settings value</returns>
		/// <exception cref="KeyNotFoundException">Key does not exist.</exception>
		Task<string> GetSettings(string key);

		/// <summary>
		/// Returns all of application settings.
		/// </summary>
		/// <returns>Collection of all application settings</returns>
		Task<IReadOnlyCollection<SettingsValue>> GetAllSettings();

		/// <summary>
		/// Sets which typology of connections are allowed to be exported
		/// </summary>
		/// <param name="typologyCodes"></param>
		/// <returns></returns>
		Task SetAllowedTypology (IEnumerable<string> typologyCodes);
	}
}