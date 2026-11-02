using System.Reflection;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Result base abstract class
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[XmlInclude(typeof(ResultOnSection))]
	public abstract class ResultBase
	{
	}
}