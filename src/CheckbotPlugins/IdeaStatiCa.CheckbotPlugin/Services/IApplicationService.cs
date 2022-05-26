using IdeaStatiCa.CheckbotPlugin.Models;
using System.Collections.Generic;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public interface IApplicationService
	{
		string GetSettings(string key);

		IReadOnlyCollection<SettingsValue> GetAllSettings();
	}
}