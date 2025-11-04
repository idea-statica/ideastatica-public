using IdeaStatiCa.Api.RCS.Model;
using Newtonsoft.Json;
using RcsApiWpfClientApp.ViewModels;
using RcsApiWpfClientApp.Views;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace RcsApiWpfClientApp.Services
{
	public interface IProjectSettingsSetter
	{
		Task<List<RcsSetting>> SetAsync(List<RcsSetting> originalSettingsValues);
	}

	internal class ProjectSettingsSetter : IProjectSettingsSetter
	{
		private readonly static JsonSerializerSettings _jsonSettings;

		static ProjectSettingsSetter()
		{
			_jsonSettings = new JsonSerializerSettings();
		}

		public async Task<List<RcsSetting>> SetAsync(List<RcsSetting> originalSettingsValues)
		{
			var defaultConversionsJson = JsonConvert.SerializeObject(originalSettingsValues, Formatting.Indented, _jsonSettings);

			JsonEditorWindow jsonEditorWindow = new JsonEditorWindow();
			jsonEditorWindow.Owner = Application.Current.MainWindow;
			JsonEditorViewModel jsonEditorViewModel = new JsonEditorViewModel();
			jsonEditorViewModel.EditedText = defaultConversionsJson;
			jsonEditorWindow.DataContext = jsonEditorViewModel;
			jsonEditorWindow.ShowDialog();

			if (jsonEditorWindow.DialogResult != true)
			{
				return await Task.FromResult(new List<RcsSetting>());
			}

			var modifiedSettings = JsonConvert.DeserializeObject<List<RcsSetting>>(jsonEditorViewModel.EditedText, _jsonSettings);
			if(modifiedSettings == null)
			{
				throw new System.Exception("Failed to deserialize defaultConversions");
			}

			return await Task.FromResult(modifiedSettings);
		}
	}
}
