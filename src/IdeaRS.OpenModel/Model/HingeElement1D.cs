namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Type of hinge in the direction
	/// </summary>
	public enum HingeTypeInDirrection
	{
		/// <summary>
		/// No hinge defined
		/// </summary>
		NoHinge = 0,

		/// <summary>
		/// Full hinge, zero stiffness is aplied
		/// </summary>
		FullHinge = 1,

		/// <summary>
		/// Initial stiffness is aplied
		/// </summary>
		Flexible = 2,

		/// <summary>
		/// Nonlinear function is aplied
		/// </summary>
		Nonlinear = 3
	}

	/// <summary>
	/// Representation of hinge element 1D
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.HingeElement1D,CI.StructuralElements", "CI.StructModel.Structure.IHingeElement1D,CI.BasicTypes")]
	public class HingeElement1D : OpenElementId
	{
		/// <summary>
		/// Initial stiffness X dirrection
		/// </summary>
		public double InitialStiffnessX { get; set; }

		/// <summary>
		/// Initial stiffness Y dirrection
		/// </summary>
		public double InitialStiffnessY { get; set; }

		/// <summary>
		/// Initial stiffness Z dirrection
		/// </summary>
		public double InitialStiffnessZ { get; set; }

		/// <summary>
		/// Initial stiffness rotational RX
		/// </summary>
		public double InitialStiffnessRX { get; set; }

		/// <summary>
		/// Initial stiffness rotational RY
		/// </summary>
		public double InitialStiffnessRY { get; set; }

		/// <summary>
		/// Initial stiffness rotational RZ
		/// </summary>
		public double InitialStiffnessRZ { get; set; }

		/// <summary>
		/// Hinge type in dirrection X
		/// </summary>
		public HingeTypeInDirrection HingeTypeX { get; set; }

		/// <summary>
		/// Hinge type in dirrection Y
		/// </summary>
		public HingeTypeInDirrection HingeTypeY { get; set; }

		/// <summary>
		/// Hinge type in dirrection Z
		/// </summary>
		public HingeTypeInDirrection HingeTypeZ { get; set; }

		/// <summary>
		/// Hinge type in dirrection round X
		/// </summary>
		public HingeTypeInDirrection HingeTypeRX { get; set; }

		/// <summary>
		/// Hinge type in dirrection round Y
		/// </summary>
		public HingeTypeInDirrection HingeTypeRY { get; set; }

		/// <summary>
		/// Hinge type in dirrection round Z
		/// </summary>
		public HingeTypeInDirrection HingeTypeRZ { get; set; }
	}
}