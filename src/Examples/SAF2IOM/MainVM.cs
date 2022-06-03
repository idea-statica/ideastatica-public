using System.ComponentModel;
using System.Windows.Input;

namespace SAF2IOM
{
	public class MainVM : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public MainVM()
		{
			this.OpenSafCommand = new CustomCommand(CanOpenSaf, OpenSaf);
			this.SaveIomCommand = new CustomCommand(CanSaveIom, SaveIom);
		}

		public ICommand OpenSafCommand {get;set;}
		public ICommand SaveIomCommand { get; set; }

		private string iom;
		public string IOM
		{
			get { return iom; }
			set
			{
				iom = value;
				NotifyPropertyChanged("IOM");
			}
		}

		private bool CanOpenSaf(object param)
		{
			return string.IsNullOrEmpty(IOM);
		}

		private void OpenSaf(object param)
		{

		}

		private bool CanSaveIom(object param)
		{
			return !string.IsNullOrEmpty(IOM);
		}

		private void SaveIom(object param)
		{

		}

		private void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
