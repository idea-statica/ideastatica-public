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
		Task<string> GetSettings(string key);

		Task<IReadOnlyCollection<SettingsValue>> GetAllSettings();
	}
}