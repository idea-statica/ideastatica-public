using IdeaStatiCa.CheckbotPlugin.Models;
using System.Collections.Generic;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	/// <summary>
	/// Provides application settings (units, language, etc.).
	/// </summary>
	public interface IApplicationService
	{
		string GetSettings(string key);

		IReadOnlyCollection<SettingsValue> GetAllSettings();
	}
}