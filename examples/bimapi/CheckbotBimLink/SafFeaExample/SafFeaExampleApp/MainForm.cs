using IdeaStatiCa.Plugin;
using SafFeaExampleLink;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SafFeaExampleApp
{
	/// <summary>
	/// The window of the example FEA application. It starts Checkbot, and lets the user change the model.
	/// </summary>
	internal sealed class MainForm : Form
	{
		private static readonly string ProjectPath = Path.Combine(AppContext.BaseDirectory, "SafFeaExampleCheckbotProject");

		private readonly FakeFeaApplication _fea;
		private readonly IPluginLogger _logger;

		private readonly TextBox _checkbotPath = new TextBox
		{
			Text = @"C:\Program Files\IDEA StatiCa\StatiCa 26.1\IdeaCheckbot.exe",
			Width = 480,
		};

		private readonly Button _startCheckbot = new Button { Text = "Start Checkbot", AutoSize = true };

		private readonly CheckBox _smallerCrossSections = new CheckBox
		{
			Text = "Smaller cross-sections (SHS100/100/6.0)",
			AutoSize = true,
		};

		public MainForm(FakeFeaApplication fea, IPluginLogger logger)
		{
			_fea = fea;
			_logger = logger;

			Text = "SAF FEA Example";
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			StartPosition = FormStartPosition.CenterScreen;
			Padding = new Padding(12);

			var layout = new FlowLayoutPanel
			{
				FlowDirection = FlowDirection.TopDown,
				WrapContents = false,
				AutoSize = true,
				Dock = DockStyle.Fill,
			};
			layout.Controls.Add(new Label { Text = "Path to IdeaCheckbot.exe:", AutoSize = true });
			layout.Controls.Add(_checkbotPath);
			layout.Controls.Add(_startCheckbot);
			layout.Controls.Add(_smallerCrossSections);
			Controls.Add(layout);

			_startCheckbot.Click += OnStartCheckbotClick;
			_smallerCrossSections.CheckedChanged += (sender, e) => _fea.SmallerCrossSections = _smallerCrossSections.Checked;
		}

		private async void OnStartCheckbotClick(object sender, EventArgs e)
		{
			string checkbotPath = _checkbotPath.Text;

			_startCheckbot.Enabled = false;
			try
			{
				await Task.Run(() => CheckbotLink.Run(checkbotPath, ProjectPath, _fea, _logger));
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				_startCheckbot.Enabled = true;
			}
		}
	}
}
