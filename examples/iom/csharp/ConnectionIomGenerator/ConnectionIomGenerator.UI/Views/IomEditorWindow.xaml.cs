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
		private readonly IomEditorWindowViewModel _viewModel;

		public IomEditorWindow(IomEditorWindowViewModel vm)
		{
			_viewModel = vm;
			this.DataContext = vm;
			InitializeComponent();

			// Subscribe to close request
			_viewModel.CloseRequested += OnCloseRequested;
		}


		private void OnCloseRequested(object? sender, DialogCloseRequestedEventArgs e)
		{
			DialogResult = e.DialogResult;
			Close();
		}

		protected override void OnClosed(System.EventArgs e)
		{
			// Unsubscribe from event
			_viewModel.CloseRequested -= OnCloseRequested;
			base.OnClosed(e);
		}
	}
}
