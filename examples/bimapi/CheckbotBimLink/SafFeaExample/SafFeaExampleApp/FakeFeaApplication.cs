using SafFeaExampleLink;
using System;
using System.Collections.Generic;
using System.IO;

namespace SafFeaExampleApp
{
	/// <summary>
	/// The example FEA application. Its model is a steel truss, which it already has as SAF files next to the
	/// application: one as designed, and one with smaller cross-sections. A real application writes the SAF file from
	/// its model instead.
	/// </summary>
	internal sealed class FakeFeaApplication : ISafExport
	{
		private static readonly string SafFilesFolder = Path.Combine(AppContext.BaseDirectory, "SafFiles");

		/// <summary>
		/// Switches the model to the truss with smaller cross-sections, as if the user had changed it in the
		/// application. Checkbot sees the change at the next sync.
		/// </summary>
		public bool SmallerCrossSections { get; set; }

		/// <summary>
		/// Saves the whole truss: the user is taken to have selected all of it.
		/// </summary>
		public void ExportSelection(string safFilePath) => SaveModel(safFilePath);

		/// <summary>
		/// Saves the whole truss, which holds every object Checkbot asks for. A real application saves only the
		/// objects with these ids.
		/// </summary>
		public void ExportObjects(string safFilePath, IReadOnlyCollection<Guid> safIds) => SaveModel(safFilePath);

		private void SaveModel(string safFilePath)
		{
			string model = SmallerCrossSections ? "SteelTrussSmallerSections.xlsx" : "SteelTruss.xlsx";
			File.Copy(Path.Combine(SafFilesFolder, model), safFilePath, overwrite: true);
		}
	}
}
