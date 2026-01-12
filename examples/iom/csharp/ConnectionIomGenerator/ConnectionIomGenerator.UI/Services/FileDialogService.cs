using Microsoft.Win32;

namespace ConnectionIomGenerator.UI.Services
{
	/// <summary>
	/// Implementation of file dialog service using WPF dialogs
	/// </summary>
	public class FileDialogService : IFileDialogService
	{
		public string? ShowSaveFileDialog(string filter, string defaultExt, string defaultFileName, string title)
		{
			var dialog = new SaveFileDialog
			{
				Filter = filter,
				DefaultExt = defaultExt,
				FileName = defaultFileName,
				Title = title
			};

			return dialog.ShowDialog() == true ? dialog.FileName : null;
		}

		public string? ShowOpenFileDialog(string filter, string title)
		{
			var dialog = new OpenFileDialog
			{
				Filter = filter,
				Title = title
			};

			return dialog.ShowDialog() == true ? dialog.FileName : null;
		}
	}
}