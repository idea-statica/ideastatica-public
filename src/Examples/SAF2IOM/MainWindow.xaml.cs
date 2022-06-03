using System.Windows;

namespace SAF2IOM
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var vm = new MainVM();
			this.DataContext = vm;
		}
	}
}
