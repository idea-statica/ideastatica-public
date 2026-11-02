using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Bonded tendon
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.BondedTendon,CI.StructuralElements", "CI.StructModel.Structure.IBondedTendon,CI.BasicTypes", typeof(Tendon))]
	public class BondedTendon : Tendon
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public BondedTendon()
		{
			//tady bude geometrie
		}
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
