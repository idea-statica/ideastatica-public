using ConApiWpfClientApp.ViewModels;
using ConApiWpfClientApp.Views;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ConApiWpfClientApp.Services
{
	internal class TextEditorService
	{


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
