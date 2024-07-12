using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Template;
using Newtonsoft.Json;
using System.Threading.Tasks;
using IdeaStatiCa.Plugin.Utilities;
using ConnectionWebClient.Views;
using System.Windows;
using ConnectionWebClient.ViewModels;

namespace ConnectionWebClient.Services
{
	public interface ITemplateMappingSetter
	{
		Task<TemplateConversions> SetAsync(TemplateConversions defaultConversions);
	}


	internal class TemplateMappingSetter : ITemplateMappingSetter
	{
		private readonly static JsonSerializerSettings _jsonSettings;

		static TemplateMappingSetter()
		{
			_jsonSettings = JsonTools.CreateIdeaRestJsonSettings();
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
