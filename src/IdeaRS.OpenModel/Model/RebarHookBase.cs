using System.Xml.Serialization;
using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a base class for rebar hook in 3D.
	/// </summary>
	[XmlInclude(typeof(RebarHookBend))]
	public abstract class RebarHookBase : OpenObject
	{
		/// <summary>
		/// Gets or sets the position (direction) of the hook in 3D. 
		/// It representa a Local Coordinate System.
		/// Hook lie in X-Y Plane. AxisX is defined by  first/last segment of the rebar. AxisX and AxisY creates a plane where the hook lies.
		/// </summary>
		public Vector3D AxisY { get; set; }

		/// <summary>
		/// Reverse direction of the hook
		/// </summary>
		public bool Reverse { get; set; }
	}
}