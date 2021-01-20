using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a rebar grouping in 3D space.
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.RebarStirrups,CI.StructuralElements", "CI.StructModel.Structure.IRebarStirrups,CI.BasicTypes")]
	public class RebarStirrups : RebarBase
	{
		/// <summary>
		/// create a new instance.
		/// </summary>
		public RebarStirrups()
		{
			Patterns = new List<RebarStirrupPattern>();
		}

		/// <summary>
		/// Gets or sets the pattern of the rebar grouping.
		/// Holds distance between rebars/stirrups and length
		/// </summary>
		public List<RebarStirrupPattern> Patterns{ get; set; }
	}
}