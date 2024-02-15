using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RcsApiClient.ViewModels
{
	public class SettingsViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private string _value;
		public string Value
		{
			get { return _value; }
			set
			{
				_value = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}
		}

		private int _selectedIndex;
		public int SelectedIndex
		{
			get { return _selectedIndex; }
			set
			{
				_selectedIndex = value;
				Value = _selectedIndex switch
				{
					3 => "{ \"value1\" : 3 , \"value2\" : 6 }",
					4 => " { \"value1\" : true , \"value2\" : 6 }",
					_ => ""
				};
				OnPropertyChanged(nameof(SelectedIndex));
			}
		}

		public string Type
		{
			get
			{
				return SelectedIndex switch
				{
					0 => "System.String",
					1 => "System.Boolean",
					2 => "System.Double",
					3 => "CI.Services.Setup.Setup2Values`2[System.Double,System.Double]",
					4 => "CI.Services.Setup.Setup2Values`2[System.Boolean,System.Double]",
					_ => "System.String"
				};
			}
		}

		private string _id;
		public string Id
		{
			get { return _id; }
			set
			{
				_id = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
			}
		}

		public ICommand OkCommand { get; }
		public ICommand CancelCommand { get; }

		public SettingsViewModel()
		{
			OkCommand = new RelayCommand(Ok, CanOk);
			CancelCommand = new RelayCommand(Cancel, CanCancel);
		}

		private bool CanCancel()
		{
			return true;
		}

		private bool CanOk()
		{
			return true;
		}

		private void Ok()
		{
			var settingsWindow = Application.Current.Windows
				.OfType<Window>()
				.FirstOrDefault(window => window.DataContext == this);

			if (settingsWindow != null 
				&& !string.IsNullOrEmpty(Id)
				&& int.TryParse(Id, out _)
				&& !string.IsNullOrEmpty(Value))
			{
				// Implement logic for OK button action (save settings, etc.)
				settingsWindow.DialogResult = true;
			}
		}

		private void Cancel()
		{
			var settingsWindow = Application.Current.Windows
				.OfType<Window>()
				.FirstOrDefault(window => window.DataContext == this);

			if (settingsWindow != null)
			{
				// Implement logic for Cancel button action
				settingsWindow.Close();
			}
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
