using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FEAppExample_1
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InvertBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool original = (bool)value;
			return !original;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool original = (bool)value;
			return !original;
		}
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly FEAppExample_1VM vm;
		private ScrollViewer scrollViewer;

		public MainWindow()
		{
			InitializeComponent();
			this.vm = new FEAppExample_1VM();
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