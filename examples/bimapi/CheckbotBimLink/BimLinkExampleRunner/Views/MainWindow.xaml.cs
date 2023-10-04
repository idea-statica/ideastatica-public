using BimLinkExampleRunner.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BimLinkExampleRunner.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			MainWindowViewModel viewModel = new MainWindowViewModel();
			DataContext = viewModel;
			viewModel.Logger.Messages.CollectionChanged += (_, __) =>
			{
				if (VisualTreeHelper.GetChildrenCount(listView) > 0)
				{
					Border border = (Border)VisualTreeHelper.GetChild(listView, 0);
					ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
					scrollViewer.ScrollToBottom();
				}
			};
		}
	}
}