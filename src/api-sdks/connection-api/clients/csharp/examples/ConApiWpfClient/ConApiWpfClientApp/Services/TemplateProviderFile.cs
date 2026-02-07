using ConApiWpfClientApp.Models;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	/// <summary>
	/// Template provider that loads a connection template from a .conTemp file selected by the user.
	/// </summary>
	internal class TemplateProviderFile : ITemplateProvider
	{
		/// <summary>
		/// Opens a file dialog for the user to select a .conTemp template file and reads its content.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project (unused, required by interface).</param>
		/// <param name="connectionId">The identifier of the connection (unused, required by interface).</param>
		/// <param name="cts">A cancellation token to cancel the operation.</param>
		/// <returns>A <see cref="ConnectionLibraryModel"/> containing the template XML,
		/// or <see langword="null"/> if the user cancelled the file dialog.</returns>
		public async Task<ConnectionLibraryModel?> GetTemplateAsync(Guid projectId, int connectionId, CancellationToken cts)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Connection Template | *.conTemp";
			if (openFileDialog.ShowDialog() != true)
			{
				return null;
			}

			var templateXml = await System.IO.File.ReadAllTextAsync(openFileDialog.FileName);

			var res = new ConnectionLibraryModel();
			res.SelectedTemplateXml = templateXml;
			return res;
		}
	}
}
