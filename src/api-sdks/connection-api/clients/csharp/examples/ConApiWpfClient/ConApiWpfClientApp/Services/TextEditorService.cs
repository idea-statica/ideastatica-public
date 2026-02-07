using ConApiWpfClientApp.ViewModels;
using ConApiWpfClientApp.Views;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ConApiWpfClientApp.Services
{
	/// <summary>
	/// Service that opens a text editor dialog for the user to edit a plain text string.
	/// Uses the JSON editor window as a general-purpose text editor.
	/// </summary>
	internal class TextEditorService
	{

		/// <summary>
		/// Opens a text editor dialog pre-populated with the given text and returns the edited result.
		/// </summary>
		/// <param name="originalInstance">The initial text to display in the editor.</param>
		/// <returns>The edited text, or <see cref="string.Empty"/> if the user cancelled the dialog.</returns>
		public async Task<string> EditAsync(string originalInstance)
		{
			JsonEditorWindow jsonEditorWindow = new JsonEditorWindow();
			jsonEditorWindow.Owner = Application.Current.MainWindow;
			JsonEditorViewModel jsonEditorViewModel = new JsonEditorViewModel();
			jsonEditorViewModel.EditedText = originalInstance;
			jsonEditorWindow.DataContext = jsonEditorViewModel;
			jsonEditorWindow.ShowDialog();


			if (jsonEditorWindow.DialogResult != true)
			{

				return await Task.FromResult(string.Empty);
			}


			return await Task.FromResult(jsonEditorViewModel.EditedText);
		}

	}
}
