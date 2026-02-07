using ConApiWpfClientApp.ViewModels;
using ConApiWpfClientApp.Views;
using IdeaStatiCa.Api.Connection.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ConApiWpfClientApp.Services
{
	/// <summary>
	/// Provides the ability to edit template conversion mappings (member and cross-section assignments)
	/// before applying a template to a connection.
	/// </summary>
	public interface ITemplateMappingSetter
	{
		/// <summary>
		/// Opens a JSON editor for the user to modify the template conversion mappings.
		/// </summary>
		/// <param name="defaultConversions">The default conversion mappings to edit.</param>
		/// <returns>The modified conversions, or <see langword="null"/> if the user cancelled.</returns>
		Task<TemplateConversions?> SetAsync(TemplateConversions defaultConversions);
	}

	/// <summary>
	/// Default implementation of <see cref="ITemplateMappingSetter"/> that presents a JSON editor
	/// dialog for modifying template conversion mappings.
	/// </summary>
	internal class TemplateMappingSetter : ITemplateMappingSetter
	{
		private readonly static JsonSerializerSettings _jsonSettings;

		static TemplateMappingSetter()
		{
			_jsonSettings = IdeaStatiCa.Api.Utilities.JsonTools.CreateIdeaRestJsonSettings();
		}

		/// <inheritdoc/>
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
