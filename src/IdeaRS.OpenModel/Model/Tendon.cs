using IdeaRS.OpenModel.Geometry3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Tendon base class
	/// </summary>
	[XmlInclude(typeof(BondedTendon))]
	[XmlInclude(typeof(PretensionedTendonGroup))]
	public abstract class Tendon : OpenElementId
	{
		/// <summary>
		/// Material
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// Number of strands
		/// </summary>
		public int NumberOfStrand { get; set; }

		///// <summary>
		///// Load case
		///// </summary>
		//public ReferenceElement LoadCase { get; set; }

		///// <summary>
		///// Geometry
		///// </summary>
		//public Polygon3D Geometry { get; set; }
	}
}
