using ConnectionIomGenerator.UI.Models;
using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.ViewModels;
using IdeaStatiCa.Plugin;
using System.Windows;

namespace ConnectionIomGenerator.UI.Views
{
	/// <summary>
	/// Interaction logic for IomEditorWindow.xaml
	/// </summary>
	public partial class IomEditorWindow : Window
	{
		public IomEditorWindow()
		{
			InitializeComponent();
		}

		private void Button_Click_Ok(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			Close();
		}

		private void Button_Click_Cancel(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}
	}
}
