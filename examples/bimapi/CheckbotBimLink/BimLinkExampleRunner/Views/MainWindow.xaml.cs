using BimLinkExampleRunner.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace BimLinkExampleRunner.Views
{
	/// <summary>
	/// The window of the runner: choose a link, start it, and read its log.
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			MainWindowViewModel viewModel = new MainWindowViewModel();
			DataContext = viewModel;
			viewModel.Logger.Messages.CollectionChanged += OnMessagesChanged;
		}

		/// <summary>
		/// Keeps the newest message in view.
		/// </summary>
		private void OnMessagesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				messages.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
			}
		}

		/// <summary>
		/// Ctrl+C copies the selected message with its exception, for example to report a problem.
		/// </summary>
		private void OnCopyMessage(object sender, ExecutedRoutedEventArgs e)
		{
			if (messages.SelectedItem != null)
			{
				Clipboard.SetText(messages.SelectedItem.ToString());
			}
		}
	}
}
