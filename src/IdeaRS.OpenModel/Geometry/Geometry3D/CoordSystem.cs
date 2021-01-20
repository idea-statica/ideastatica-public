using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Coordinate system base class
	/// </summary>
	[XmlInclude(typeof(CoordSystemByPoint))]
	[XmlInclude(typeof(CoordSystemByVector))]
	[XmlInclude(typeof(CoordSystemByZup))]
	public abstract class CoordSystem : OpenObject
	{
	}
}