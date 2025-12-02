using Microsoft.Win32;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	internal class TemplateProviderFile : ITemplateProvider
	{
		public async Task<string> GetTemplateAsync()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Connection Template | *.conTemp";
			if (openFileDialog.ShowDialog() != true)
			{
				return string.Empty;
			}

			var templateXml = await System.IO.File.ReadAllTextAsync(openFileDialog.FileName);
			return templateXml;
		}
	}
}
