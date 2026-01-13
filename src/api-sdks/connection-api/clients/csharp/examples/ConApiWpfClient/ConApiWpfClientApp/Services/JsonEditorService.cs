using ConApiWpfClientApp.ViewModels;
using ConApiWpfClientApp.Views;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ConApiWpfClientApp.Services
{
	internal class JsonEditorService<T>
	{
		private readonly static JsonSerializerSettings _jsonSettings;

		static JsonEditorService()
		{
			_jsonSettings = IdeaStatiCa.Api.Utilities.JsonTools.CreateIdeaRestJsonSettings();
		}

		public async Task<T?> EditAsync(T originalInstance)
		{
			var defaultConversionsJson = JsonConvert.SerializeObject(originalInstance, Formatting.Indented, _jsonSettings);

			JsonEditorWindow jsonEditorWindow = new JsonEditorWindow();
			jsonEditorWindow.Owner = Application.Current.MainWindow;
			JsonEditorViewModel jsonEditorViewModel = new JsonEditorViewModel();
			jsonEditorViewModel.EditedText = defaultConversionsJson;
			jsonEditorWindow.DataContext = jsonEditorViewModel;
			jsonEditorWindow.ShowDialog();

			T? editedInstance = default(T);

			if (jsonEditorWindow.DialogResult != true)
			{

				return await Task.FromResult(editedInstance);
			}

			editedInstance = JsonConvert.DeserializeObject<T>(jsonEditorViewModel.EditedText, _jsonSettings);
			if (editedInstance == null)
			{
				throw new System.Exception("Failed to deserialize T");
			}

			return await Task.FromResult(editedInstance);
		}

	}
}
