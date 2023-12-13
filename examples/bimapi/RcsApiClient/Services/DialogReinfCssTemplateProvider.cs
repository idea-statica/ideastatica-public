using Google.Protobuf;
using IdeaStatiCa.RcsClient.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;

namespace RcsApiClient.Services
{
	public class DialogReinfCssTemplateProvider : IReinfCssTemplateProvider
	{
		public string GetTemplate()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			// Set properties for the OpenFileDialog
			openFileDialog.Title = "Select a reinforced cross-section template";
			openFileDialog.Filter = "Reinforced Css template (*.nav)|*.nav";


			// Show the file dialog and get the selected file
			if (openFileDialog.ShowDialog() == true)
			{
				StringReader stringReader = new StringReader(openFileDialog.FileName)

			}
			else
			{
				return string.Empty;
			}
		}
	}
}
