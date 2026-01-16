using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel;
using System.Threading.Tasks;

namespace ConnectionIomGenerator.UI.Services
{
	/// <summary>
	/// Service for IOM generation and serialization operations.
	/// Provides methods to create, serialize, and persist IDEA StatiCa OpenModel (IOM) data structures
	/// used for structural analysis and connection design.
	/// </summary>
	public interface IIomService
	{
		/// <summary>
		/// Generates an OpenModelContainer from connection input parameters.
		/// Creates a complete structural model including geometry, materials, cross-sections,
		/// members, and optionally loading conditions for IDEA StatiCa analysis.
		/// </summary>
		/// <param name="input">The connection input containing material specification and geometric parameters.</param>
		/// <param name="loadingInput">Optional loading conditions to be applied to the connection model. 
		/// If null, the model is generated without load cases.</param>
		/// <returns>A task that represents the asynchronous operation. 
		/// The task result contains an OpenModelContainer with both structural data (OpenModel) 
		/// and results placeholder (OpenModelResult).</returns>
		/// <remarks>
		/// The generated OpenModelContainer includes:
		/// - OpenModel: Structural geometry, materials, cross-sections, members, and loads
		/// - OpenModelResult: Results placeholder for finite element analysis output
		/// </remarks>
		Task<OpenModelContainer> GenerateIomAsync(ConnectionInput input, LoadingInput? loadingInput);

		/// <summary>
		/// Serializes an OpenModel object to its XML string representation.
		/// Converts the structural model data into IDEA StatiCa XML format for storage or exchange.
		/// </summary>
		/// <param name="openModel">The OpenModel instance to serialize. Must not be null.</param>
		/// <returns>A string containing the XML representation of the OpenModel, 
		/// compatible with IDEA StatiCa import and analysis tools.</returns>
		/// <remarks>
		/// The XML format follows the IDEA StatiCa OpenModel schema and can be:
		/// - Saved to a .xml file for later use
		/// - Imported into IDEA StatiCa applications
		/// - Exchanged between different structural analysis tools
		/// </remarks>
		string SerializeToXml(OpenModel openModel);

		/// <summary>
		/// Saves an OpenModelContainer to a file on disk.
		/// Persists both the structural model and analysis results to the specified file path.
		/// </summary>
		/// <param name="container">The OpenModelContainer to save, containing OpenModel and OpenModelResult data.</param>
		/// <param name="filePath">The full path where the file should be saved. 
		/// The directory must exist and the application must have write permissions.
		/// Typically uses .ideaCon or .xml file extension.</param>
		/// <returns>A task that represents the asynchronous file write operation.</returns>
		/// <remarks>
		/// The saved file can be:
		/// - Opened in IDEA StatiCa Connection application
		/// - Used as input for automated analysis workflows
		/// - Shared with other users or systems
		/// </remarks>
		/// <exception cref="IOException">Thrown when the file path is invalid or inaccessible.</exception>
		/// <exception cref="UnauthorizedAccessException">Thrown when write permissions are insufficient.</exception>
		Task SaveToFileAsync(OpenModelContainer container, string filePath);
	}
}