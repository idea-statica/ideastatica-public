using System.Windows;

namespace IdeaStatiCaFake
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private IdeaStatiCaFakeVM vm;

		public MainWindow()
		{
			InitializeComponent();
			vm = new IdeaStatiCaFakeVM();
			DataContext = vm;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (vm != null)
			{
				vm.Dispose();
			}
		}
	}
}