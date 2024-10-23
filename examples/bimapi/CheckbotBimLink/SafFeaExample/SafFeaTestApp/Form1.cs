using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using SafFeaApi_MOCK;
using SafFeaBimLink;
using Serilog;

namespace SafFeaTestApp
{
	public partial class Form1 : Form
	{
		FeaModelApiClient _modelClient;

		public Form1()
		{
			InitializeComponent();
			_modelClient = new FeaModelApiClient();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Task.Run(() => SafFeaBimLinkApp.Run(_modelClient));
		}
	}
}
