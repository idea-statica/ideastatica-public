using IdeaRS.OpenModel.Geometry3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// The Pretensioned tendon group
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.PretensionedTendonGroup,CI.StructuralElements", "CI.StructModel.Structure.IPretensionedTendonGroup,CI.BasicTypes", typeof(Tendon))]
	public class PretensionedTendonGroup : Tendon
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PretensionedTendonGroup()
		{
			Items = new List<PretensionedTendonGroupItem>();
		}
		/// <summary>
		/// List of the pretensioned tendon in the group
		/// </summary>
		public List<PretensionedTendonGroupItem> Items { get; set; }
	}

	/// <summary>
	/// The pretensionede tendon group item
	/// </summary>
	//[OpenModelClass("CI.StructModel.Structure.PretensionedTendonGroupItem,CI.StructuralElements", "CI.StructModel.Structure.IPretensionedTendonGroupItem,CI.BasicTypes")]
	public class PretensionedTendonGroupItem : Tendon
	{
		/// <summary>
		/// Geometry
		/// </summary>
		public Polygon3D Geometry { get; set; }

		///// <summary>
		///// Material
		///// </summary>
		//public ReferenceElement Material { get; set; }

		///// <summary>
		///// Number of strands
		///// </summary>
		//public int NumberOfStrand { get; set; }
	}
}
