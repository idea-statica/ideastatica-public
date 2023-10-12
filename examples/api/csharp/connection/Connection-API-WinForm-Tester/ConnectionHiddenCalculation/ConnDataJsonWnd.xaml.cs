using System;
using System.Windows;

namespace ConnectionHiddenCalculation
{
	/// <summary>
	/// Interaction logic for ConcParamsWnd.xaml
	/// </summary>
	public partial class ConnDataJsonWnd : Window
	{
		private readonly ConnDataJsonVM viewModel;

		public ConnDataJsonWnd()
		{
			InitializeComponent();
		}

		public ConnDataJsonWnd(ConnDataJsonVM viewModel) : this()
		{
			this.viewModel = viewModel;
			this.viewModel.UpdateFinished += ViewModel_UpdateFinished;
			DataContext = viewModel;
		}

		private void ViewModel_UpdateFinished(object sender, EventArgs e)
		{
			Dispatcher.BeginInvoke((Action)(() =>
			{
				DialogResult = true;
				Close();
			}));
		}
	}
}
