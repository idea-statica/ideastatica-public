using IdeaStatiCa.ConRestApiClientUI;
using System.Windows;

namespace ConApiWpfClientApp
{
	/// <summary>
	/// Main application window that hosts the Connection API client UI,
	/// including the 3D scene viewer and command panels.
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly IConRestApiClientViewModel _conRestApiClientViewModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		/// <param name="conRestApiClientViewModel">The view model providing 3D scene rendering capabilities.</param>
		public MainWindow(IConRestApiClientViewModel conRestApiClientViewModel)
		{
			_conRestApiClientViewModel = conRestApiClientViewModel;
			InitializeComponent();

			Scene3DHostControl.Children.Add(new IdeaWebGlScene3D(_conRestApiClientViewModel));
		}
	}
}
