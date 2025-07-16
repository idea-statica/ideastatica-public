using IdeaStatiCa.ConRestApiClientUI;
using System.Windows;

namespace ConApiWpfClientApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly IConRestApiClientViewModel _conRestApiClientViewModel;

		public MainWindow(IConRestApiClientViewModel conRestApiClientViewModel)
		{
			_conRestApiClientViewModel = conRestApiClientViewModel;
			InitializeComponent();

			Scene3DHostControl.Children.Add(new IdeaWebGlScene3D(_conRestApiClientViewModel));
		}
	}
}
