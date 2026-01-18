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
		private readonly IomEditorViewModel _viewModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="IomEditorWindow"/> class.
		/// </summary>
		/// <param name="logger">Logger for tracking operations.</param>
		/// <param name="iomService">Service for IOM operations.</param>
		/// <param name="fileDialogService">Service for file dialog operations.</param>
		public IomEditorWindow(
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService)
		{
			InitializeComponent();
			
			_viewModel = new IomEditorViewModel(logger, iomService, fileDialogService);
			DataContext = _viewModel;
			
			// Subscribe to close request
			_viewModel.CloseRequested += OnCloseRequested;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IomEditorWindow"/> class with initial model.
		/// </summary>
		/// <param name="model">Initial model data.</param>
		/// <param name="logger">Logger for tracking operations.</param>
		/// <param name="iomService">Service for IOM operations.</param>
		/// <param name="fileDialogService">Service for file dialog operations.</param>
		public IomEditorWindow(
			IomGeneratorModel model,
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService)
		{
			InitializeComponent();
			
			_viewModel = new IomEditorViewModel(model, logger, iomService, fileDialogService);
			DataContext = _viewModel;
			
			// Subscribe to close request
			_viewModel.CloseRequested += OnCloseRequested;
		}

		/// <summary>
		/// Gets the result model after OK is clicked.
		/// The model is only updated when user clicks OK.
		/// </summary>
		public IomGeneratorModel? ResultModel => DialogResult == true ? _viewModel.GetResultModel() : null;

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
