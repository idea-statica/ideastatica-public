using System.Reflection;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Point Result Base abstract class
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[XmlInclude(typeof(PointResultOfNLA))]
	[XmlInclude(typeof(PointResultOfTA))]
	public abstract class PointResultBase
	{
		/// <summary>
		/// Loading
		/// </summary>
		public ResultOfLoading Loading { get; set; }
	}
}