using ConnectionIomGenerator.UI.Models;
using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.Views;
using IdeaStatiCa.Plugin;
using System.Windows;

namespace ConnectionIomGenerator.UI.Examples
{
	/// <summary>
	/// Example usage of IomEditorWindow
	/// </summary>
	public class IomEditorWindowExample
	{
		/// <summary>
		/// Shows how to open the IomEditorWindow and get the result.
		/// </summary>
		/// <param name="logger">Logger instance.</param>
		/// <param name="iomService">IOM service instance.</param>
		/// <param name="fileDialogService">File dialog service instance.</param>
		/// <param name="owner">Owner window for the dialog.</param>
		/// <returns>The model if user clicked OK, null if cancelled.</returns>
		public static IomGeneratorModel? ShowEditorDialog(
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService,
			Window? owner = null)
		{
			// Create the dialog window
			var editorWindow = new IomEditorWindow(logger, iomService, fileDialogService)
			{
				Owner = owner
			};

			// Show as modal dialog
			bool? result = editorWindow.ShowDialog();

			// If user clicked OK, return the model
			if (result == true)
			{
				return editorWindow.ResultModel;
			}

			// User clicked Cancel
			return null;
		}

		/// <summary>
		/// Shows how to open the IomEditorWindow with initial data.
		/// </summary>
		/// <param name="initialModel">Initial model to populate the editor.</param>
		/// <param name="logger">Logger instance.</param>
		/// <param name="iomService">IOM service instance.</param>
		/// <param name="fileDialogService">File dialog service instance.</param>
		/// <param name="owner">Owner window for the dialog.</param>
		/// <returns>The updated model if user clicked OK, null if cancelled.</returns>
		public static IomGeneratorModel? ShowEditorDialogWithData(
			IomGeneratorModel initialModel,
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService,
			Window? owner = null)
		{
			// Create the dialog window with initial data
			var editorWindow = new IomEditorWindow(initialModel, logger, iomService, fileDialogService)
			{
				Owner = owner
			};

			// Show as modal dialog
			bool? result = editorWindow.ShowDialog();

			// If user clicked OK, return the updated model
			if (result == true)
			{
				return editorWindow.ResultModel;
			}

			// User clicked Cancel
			return null;
		}

		/// <summary>
		/// Example of using the editor in an application.
		/// </summary>
		public static void ExampleUsage(
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService)
		{
			// Example 1: Open empty editor
			var model1 = ShowEditorDialog(logger, iomService, fileDialogService);
			if (model1 != null)
			{
				// User clicked OK, use the model
				var connectionInput = model1.ConnectionInput;
				// ... generate IOM, etc.
			}

			// Example 2: Open editor with existing data
			var existingModel = new IomGeneratorModel();
			// ... populate existingModel with data
			
			var model2 = ShowEditorDialogWithData(existingModel, logger, iomService, fileDialogService);
			if (model2 != null)
			{
				// User clicked OK, use the updated model
				var updatedConnectionInput = model2.ConnectionInput;
				// ... generate IOM, etc.
			}
		}
	}
}
