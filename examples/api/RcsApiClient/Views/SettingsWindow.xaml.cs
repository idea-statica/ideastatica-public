using RcsApiClient.ViewModels;
using System.Windows;

namespace RcsApiClient.Views
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow(SettingsViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
