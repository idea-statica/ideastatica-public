using SafFeaApi_MOCK;
using SafFeaBimLink;

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

		private void button3_Click(object sender, EventArgs e)
		{
			string directoryPath = _modelClient.GetModelDirectory();
			Directory.Delete(directoryPath, true);
		}
	}
}
