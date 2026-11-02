using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete base class
	/// </summary>
	[XmlInclude(typeof(MatConcreteEc2))]
	[XmlInclude(typeof(MatConcreteSIA))]
	[XmlInclude(typeof(MatConcreteACI))]
	[XmlInclude(typeof(MatConcreteCAN))]
	[XmlInclude(typeof(MatConcreteAUS))]
	[XmlInclude(typeof(MatConcreteRUS))]
	[XmlInclude(typeof(MatConcreteIND))]
	[XmlInclude(typeof(MatConcreteHKG))]
	[XmlInclude(typeof(MatConcreteCHN))]
	public abstract class MatConcrete : Material
	{
	}
}