using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FEAppTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly FEAppTestVM vm;
		private ScrollViewer scrollViewer;

		public MainWindow()
		{
			InitializeComponent();
			this.vm = new FEAppTestVM();
			DataContext = vm;

			vm.Actions.CollectionChanged += OnItemsSourceChanged;
		}

		private void OnItemsSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var scrollViewer = GetScrollViewer();
			scrollViewer?.ScrollToBottom();
		}

		private ScrollViewer GetScrollViewer()
		{
			if (scrollViewer == null && VisualTreeHelper.GetChildrenCount(listView) > 0)
			{
				Border border = (Border)VisualTreeHelper.GetChild(listView, 0);
				scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
			}

			return scrollViewer;
		}
	}
}