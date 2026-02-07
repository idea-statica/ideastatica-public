using ConApiWpfClientApp.ViewModels;
using ConApiWpfClientApp.Views;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ConApiWpfClientApp.Services
{
	/// <summary>
	/// Generic service that opens a JSON editor dialog for the user to edit a serialized object
	/// and returns the deserialized result.
	/// </summary>
	/// <typeparam name="T">The type of object to edit. Must be serializable with Newtonsoft.Json.</typeparam>
	internal class JsonEditorService<T>
	{
		private readonly static JsonSerializerSettings _jsonSettings;

		static JsonEditorService()
		{
			_jsonSettings = IdeaStatiCa.Api.Utilities.JsonTools.CreateIdeaRestJsonSettings();
		}

		/// <summary>
		/// Serializes the provided instance to JSON, opens a JSON editor dialog,
		/// and deserializes the edited JSON back to an instance of <typeparamref name="T"/>.
		/// </summary>
		/// <param name="originalInstance">The object to serialize and present for editing.</param>
		/// <returns>The edited and deserialized instance, or the default value of <typeparamref name="T"/>
		/// if the user cancelled the dialog.</returns>
		/// <exception cref="System.Exception">Thrown when deserialization of the edited JSON fails.</exception>
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
