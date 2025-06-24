using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Material
{
	[XmlInclude(typeof(MatTimberEc5))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public abstract class MatTimber : Material
	{
	}
}
