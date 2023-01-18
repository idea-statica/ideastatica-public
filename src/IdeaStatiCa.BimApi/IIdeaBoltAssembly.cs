namespace IdeaStatiCa.BimApi
{
	public interface IIdeaBoltAssembly : IIdeaObject
	{
		/// <summary>
		/// Hole Diameter
		/// </summary>
		double HoleDiameter { get; }

		/// <summary>
		/// Diameter
		/// </summary>
		double Diameter { get; }

		/// <summary>
		/// Head Diamete
		/// </summary>
		double HeadDiameter { get; }

		/// <summary>
		/// Lenght of bolt
		/// </summary>
		double Lenght { get; }

		/// <summary>
		/// Diagonal head diametere
		/// </summary>
		double DiagonalHeadDiameter { get; }

		/// <summary>
		/// Head height
		/// </summary>
		double HeadHeight { get; }

		/// <summary>
		/// Bore hole
		/// </summary>
		double BoreHole { get; }

		/// <summary>
		/// Tensile Stress Area
		/// </summary>
		double TensileStressArea { get; }

		/// <summary>
		/// Nut thickness
		/// </summary>
		double NutThickness { get; }

		/// <summary>
		/// Standard
		/// </summary>
		string Standard { get; }

		/// <summary>
		/// Material of the plate.
		/// </summary>
		IIdeaMaterial Material { get; }
	}
}
