namespace IdeaStatiCa.BimApi
{
	public interface IIdeaBoltAssemblyByParameters : IIdeaBoltAssembly
	{
		/// <summary>
		/// Diameter
		/// </summary>
		double Diameter { get; }

		/// <summary>
		/// Bore hole
		/// </summary>
		double BoreHole { get; }

		/// <summary>
		/// Head Diameter
		/// </summary>
		double HeadDiameter { get; }

		/// <summary>
		/// Diagonal head diameter
		/// </summary>
		double DiagonalHeadDiameter { get; }

		/// <summary>
		/// Head height
		/// </summary>
		double HeadHeight { get; }

		/// <summary>
		/// Head height
		/// </summary>
		double GrossArea { get; }

		/// <summary>
		/// Tensile Stress Area
		/// </summary>
		double TensileStressArea { get; }

		/// <summary>
		/// Nut thickness
		/// </summary>
		double NutThickness { get; }

		/// <summary>
		/// Thickness of washer
		/// </summary>
		double WasherThickness
		{
			get;
			set;
		}

		/// <summary>
		/// Washer at head side of bolt assembly
		/// </summary>
		bool WasherAtHead
		{
			get;
			set;
		}

		/// <summary>
		/// Is washer at Nut side of bolt assembly
		/// </summary>
		bool WasherAtNut
		{
			get;
			set;
		}

		/// <summary>
		/// Material of the boltassembly
		/// </summary>
		IIdeaMaterial BoltGrade { get; }
	}
}
