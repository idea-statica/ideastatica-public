using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConApiWpfClientApp.Views
{
	/// <summary>
	/// User control that displays a list of proposed connection design items
	/// from the connection library search results.
	/// </summary>
	public partial class DesignItemsView : UserControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DesignItemsView"/> class.
		/// </summary>
		public DesignItemsView()
		{
			InitializeComponent();
		}
	}
}
