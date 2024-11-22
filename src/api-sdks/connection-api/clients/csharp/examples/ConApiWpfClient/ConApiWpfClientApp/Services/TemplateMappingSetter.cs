using ConApiWpfClientApp.ViewModels;
using ConApiWpfClientApp.Views;
using IdeaStatiCa.Api.Connection.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ConApiWpfClientApp.Services
{
	public interface ITemplateMappingSetter
	{
		Task<TemplateConversions?> SetAsync(TemplateConversions defaultConversions);
	}

	internal class TemplateMappingSetter : ITemplateMappingSetter
	{
		private readonly static JsonSerializerSettings _jsonSettings;

		static TemplateMappingSetter()
		{
			_jsonSettings = IdeaStatiCa.Api.Utilities.JsonTools.CreateIdeaRestJsonSettings();
		}

		public async Task<TemplateConversions?> SetAsync(TemplateConversions defaultConversions)
		{
			var defaultConversionsJson = JsonConvert.SerializeObject(defaultConversions, Formatting.Indented, _jsonSettings);

			JsonEditorWindow jsonEditorWindow = new JsonEditorWindow();
			jsonEditorWindow.Owner = Application.Current.MainWindow;
			JsonEditorViewModel jsonEditorViewModel = new JsonEditorViewModel();
			jsonEditorViewModel.EditedText = defaultConversionsJson;
			jsonEditorWindow.DataContext = jsonEditorViewModel;
			jsonEditorWindow.ShowDialog();

			TemplateConversions? modifiedConversions = null;

			if (jsonEditorWindow.DialogResult != true)
			{

				return await Task.FromResult(modifiedConversions);
			}

			modifiedConversions = JsonConvert.DeserializeObject<TemplateConversions>(jsonEditorViewModel.EditedText, _jsonSettings);
			if(modifiedConversions == null)
			{
				throw new System.Exception("Failed to deserialize defaultConversions");
			}

			return await Task.FromResult(modifiedConversions);
		}
	}
}
