namespace ConApiWpfClientApp.ViewModels
{
	/// <summary>
	/// View model for the JSON editor dialog, providing a bindable text property for editing JSON content.
	/// </summary>
	public class JsonEditorViewModel : ViewModelBase
	{
		private string? editedText;

		/// <summary>
		/// Gets or sets the JSON text being edited.
		/// </summary>
		public string? EditedText
		{
			get => editedText;
			set
			{
				SetProperty(ref editedText, value);
			}
		}
	}
}
