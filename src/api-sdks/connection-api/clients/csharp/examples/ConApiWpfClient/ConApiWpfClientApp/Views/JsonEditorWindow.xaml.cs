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
using System.Windows.Shapes;

namespace ConApiWpfClientApp.Views
{
	/// <summary>
	/// Modal dialog window for editing JSON or plain text content.
	/// Used by <see cref="Services.JsonEditorService{T}"/> and <see cref="Services.TextEditorService"/>.
	/// </summary>
	public partial class JsonEditorWindow : Window
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonEditorWindow"/> class.
		/// </summary>
		public JsonEditorWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Handles the OK button click by setting the dialog result to <see langword="true"/> and closing the window.
		/// </summary>
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// Close the modal window
			this.DialogResult = true;
			this.Close();
		}
	}
}
