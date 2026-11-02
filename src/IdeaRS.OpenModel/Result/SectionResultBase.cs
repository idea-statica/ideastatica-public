using System.Reflection;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Section result base
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[XmlInclude(typeof(ResultOfDeformation))]
	[XmlInclude(typeof(ResultOfInternalForces))]
	[XmlInclude(typeof(SectionResultOnPoints))]
	//[XmlInclude(typeof(ResultOfInteractionDiagram))]
	[XmlInclude(typeof(ResultOfInteractionDiagramPlane))]
	//[XmlInclude(typeof(ResultOfDisplacementChart))]
	[XmlInclude(typeof(SectionResultMesh))]
	public abstract class SectionResultBase
	{
		/// <summary>
		/// Loading
		/// </summary>
		public ResultOfLoading Loading { get; set; }
	}
}