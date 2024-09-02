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

namespace ConnectionWebClient.Views
{
	/// <summary>
	/// Interaction logic for JsonEditorWindow.xaml
	/// </summary>
	public partial class JsonEditorWindow : Window
	{
		public JsonEditorWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// Close the modal window
			this.DialogResult = true;
			this.Close();
		}
	}
}
