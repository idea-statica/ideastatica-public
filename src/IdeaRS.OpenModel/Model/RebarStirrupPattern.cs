namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a pattern of rebar
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.RebarStirrupPattern,CI.StructuralElements", "CI.StructModel.Structure.IRebarStirrupPattern,CI.BasicTypes")]
	public class RebarStirrupPattern: RebarPatternBase
	{
		/// <summary>
		/// create a new instance.
		/// </summary>
		public RebarStirrupPattern()
		: base()
		{
			Translation = new IdeaRS.OpenModel.Geometry3D.Vector3D();
		}

		/// <summary>
		/// Gets or sets the distance between rebars for this pattern.
		/// </summary>
		public double Distance { get; set; }

		/// <summary>
		/// Gets or sets the local eccentricity in X and Y direction
		/// apply transfotmation at 3D (section 3D) then transform to 3D.
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D Translation { get; set; }

		/// <summary>
		/// true: add first stirrup for this pattern.
		/// used do exclude the first stirrup in patterns > first.
		/// </summary>
		public bool AddFirstStirrup	{	get; set;	}
	}
}
