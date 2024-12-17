namespace RcsApiWpfClientApp.ViewModels
{
	public class JsonEditorViewModel : ViewModelBase
	{
		private string? editedText;

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
