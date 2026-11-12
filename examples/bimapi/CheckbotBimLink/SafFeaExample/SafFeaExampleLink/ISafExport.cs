using System;
using System.Collections.Generic;

namespace SafFeaExampleLink
{
	/// <summary>
	/// What the link needs from the FEA application: its model, saved as a SAF file. A real FEA application implements
	/// it with its own SAF export.
	/// </summary>
	/// <remarks>
	/// Every object must keep its SAF id from one export to the next. The link remembers which IDEA StatiCa object
	/// came from which SAF id, and asks for the objects by these ids when the user synchronizes in Checkbot.
	/// </remarks>
	public interface ISafExport
	{
		/// <summary>
		/// Saves the objects the user has selected in the FEA application to a SAF file, with their results.
		/// </summary>
		/// <param name="safFilePath">Full path of the SAF file to write. If the file exists, overwrite it.</param>
		void ExportSelection(string safFilePath);

		/// <summary>
		/// Saves the objects with the given SAF ids to a SAF file, as they are now in the FEA application, with their
		/// results.
		/// </summary>
		/// <param name="safFilePath">Full path of the SAF file to write. If the file exists, overwrite it.</param>
		/// <param name="safIds">The SAF ids of the objects Checkbot has imported before.</param>
		void ExportObjects(string safFilePath, IReadOnlyCollection<Guid> safIds);
	}
}
