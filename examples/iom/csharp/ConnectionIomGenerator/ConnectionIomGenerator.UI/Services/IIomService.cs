using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel;
using System.Threading.Tasks;

namespace ConnectionIomGenerator.UI.Services
{
	/// <summary>
	/// Service for IOM generation and serialization operations
	/// </summary>
	public interface IIomService
	{
		/// <summary>
		/// Generates an OpenModelContainer from connection input
		/// </summary>
		Task<OpenModelContainer> GenerateIomAsync(ConnectionInput input);

		/// <summary>
		/// Serializes OpenModel to XML string
		/// </summary>
		string SerializeToXml(OpenModel openModel);

		/// <summary>
		/// Saves OpenModelContainer to a file
		/// </summary>
		Task SaveToFileAsync(OpenModelContainer container, string filePath);
	}
}