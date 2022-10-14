using BimApiExample.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BimApiExample.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			MainWindowViewModel viewModel = new();
			DataContext = viewModel;
			viewModel.Logger.Messages.CollectionChanged += (_, _) =>
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