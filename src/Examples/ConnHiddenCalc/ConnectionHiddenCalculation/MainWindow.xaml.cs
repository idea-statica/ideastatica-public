using System;
using System.Windows;

namespace ConnectionHiddenCalculation
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly MainVM vm;

		public MainWindow()
		{
			InitializeComponent();
			vm = new MainVM();
			DataContext = vm;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			vm?.CloseConnectionService();
		}
	}
}
