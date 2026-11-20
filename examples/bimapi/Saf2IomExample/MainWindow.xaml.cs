using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.SAF2IOM;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Saf2IomExample
{
	/// <summary>
	/// Converts a SAF file to IOM with the IdeaStatiCa.SAF2IOM package, shows what the IOM holds, and saves it.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// The design code the IOM is made for.
		/// </summary>
		private const CountryCode DesignCode = CountryCode.ECEN;

		private readonly IPluginLogger _logger;

		/// <summary>
		/// The IOM of the last SAF file: the model and its internal forces.
		/// </summary>
		private OpenModelContainer _iom;

		public MainWindow(IPluginLogger logger)
		{
			_logger = logger;
			InitializeComponent();
		}

		private async void OpenSaf_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog
			{
				Filter = "SAF files (*.xlsx)|*.xlsx",
				InitialDirectory = Path.Combine(AppContext.BaseDirectory, "SafFiles"),
			};
			if (dialog.ShowDialog(this) != true)
			{
				return;
			}

			string safFilePath = dialog.FileName;
			_iom = null;
			OpenSafButton.IsEnabled = false;
			SaveIomButton.IsEnabled = false;
			Output.Text = $"Converting {safFilePath}...";

			try
			{
				_iom = await Task.Run(() => Convert(safFilePath));
				Output.Text = Describe(safFilePath, _iom);
				SaveIomButton.IsEnabled = true;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Converting {safFilePath} failed", ex);
				Output.Text = $"Converting {safFilePath} failed: {ex.Message}";
			}
			finally
			{
				OpenSafButton.IsEnabled = true;
			}
		}

		/// <summary>
		/// Reads the SAF file and makes the IOM from it. SAF2IOM adds a connection at every node where two or more members
		/// meet, and at every supported node.
		/// </summary>
		private static OpenModelContainer Convert(string safFilePath)
		{
			ModelBIM model = new SAF2IOMConverter().Convert(new SafConverterInput(safFilePath, DesignCode));
			return new OpenModelContainer
			{
				OpenModel = model.Model,
				OpenModelResult = model.Results,
			};
		}

		private static string Describe(string safFilePath, OpenModelContainer iom)
		{
			OpenModel model = iom.OpenModel;
			int membersWithForces = iom.OpenModelResult.ResultOnMembers.Sum(results => results.Members.Count);

			return string.Join(Environment.NewLine,
				$"{safFilePath} is converted to IOM:",
				$"{model.Member1D.Count} members",
				$"{model.ConnectionPoint.Count} connections",
				$"{model.LoadCase.Count} load cases",
				$"{model.CombiInput.Count} load combinations",
				$"internal forces on {membersWithForces} members",
				"",
				"Click Save IOM to save it.");
		}

		/// <summary>
		/// Saves the model and its internal forces in one XML file, which the Connection API can create a project from.
		/// </summary>
		private void SaveIom_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog
			{
				Filter = "IOM files (*.xml)|*.xml",
			};
			if (dialog.ShowDialog(this) != true)
			{
				return;
			}

			try
			{
				IdeaRS.OpenModel.Tools.OpenModelContainerToFile(_iom, dialog.FileName);
				Output.Text = $"The IOM is saved to {dialog.FileName}.";
			}
			catch (Exception ex)
			{
				_logger.LogError($"Saving {dialog.FileName} failed", ex);
				Output.Text = $"Saving {dialog.FileName} failed: {ex.Message}";
			}
		}
	}
}
