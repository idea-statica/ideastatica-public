using IdeaStatiCa.RcsClient.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RcsApiClient.Services
{
	public class DialogReinfCssTemplateProvider : IReinfCssTemplateProvider
	{
		public async Task<string> GetTemplateAsync()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			// Set properties for the OpenFileDialog
			openFileDialog.Title = "Select a reinforced cross-section template";
			openFileDialog.Filter = "Reinforced Css template (*.nav)|*.nav";

			// Show the file dialog and get the selected file
			if (openFileDialog.ShowDialog() == true)
			{
				using (var sr = new StreamReader(openFileDialog.FileName))
				{
					// Read the stream as a string, and write the string to the console.
					var template = await sr.ReadToEndAsync();
					return template;
				}
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
