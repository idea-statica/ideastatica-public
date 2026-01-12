namespace ConnectionIomGenerator.UI.Services
{
	/// <summary>
	/// Service for showing file dialogs
	/// </summary>
	public interface IFileDialogService
	{
		/// <summary>
		/// Shows a save file dialog
		/// </summary>
		/// <param name="filter">File filter (e.g., "XML files (*.xml)|*.xml")</param>
		/// <param name="defaultExt">Default file extension</param>
		/// <param name="defaultFileName">Default file name</param>
		/// <param name="title">Dialog title</param>
		/// <returns>Selected file path or null if cancelled</returns>
		string? ShowSaveFileDialog(string filter, string defaultExt, string defaultFileName, string title);

		/// <summary>
		/// Shows an open file dialog
		/// </summary>
		/// <param name="filter">File filter</param>
		/// <param name="title">Dialog title</param>
		/// <returns>Selected file path or null if cancelled</returns>
		string? ShowOpenFileDialog(string filter, string title);
	}
}