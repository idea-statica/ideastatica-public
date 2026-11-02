using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Represents a region in two-dimensional space included outline (border) and openings.
	/// </summary>
	[OpenModelClass("CI.Geometry2D.Region2D,CI.Geometry2D")]
	[DataContract]
	public class Region2D : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Region2D()
		{
			Openings = new List<PolyLine2D>();
		}

		/// <summary>
		/// Gets or sets the outline curve of Region2D.
		/// </summary>
		[DataMember]
		public PolyLine2D Outline { get; set; }

		/// <summary>
		/// Gets or sets the list of openings in the Region2D.
		/// </summary>
		[XmlElement(IsNullable = true)]
		[DataMember]
		public List<PolyLine2D> Openings { get; set; }
	}
}