using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Type of curvature -  thermal expansion
	/// </summary>
	public enum ThermalExpansionState
	{
		/// <summary>
		/// The curvature is not defined
		/// </summary>
		None = 0,

		/// <summary>
		/// The curvatire is defined by the code
		/// </summary>
		Code = 1,

		/// <summary>
		/// The curvature is input by the user
		/// </summary>
		User = 5,
	}

	/// <summary>
	/// Type of curvature - thermal conductivity
	/// </summary>
	public enum ThermalConductivityState
	{
		/// <summary>
		/// The curvature is not defined
		/// </summary>
		None = 0,

		/// <summary>
		/// The curvatire is defined by the code
		/// </summary>
		Code = 1,

		/// <summary>
		/// The curvature is input by the user
		/// </summary>
		User = 5,
	}

	/// <summary>
	/// Type of curvature - thermal specific heat
	/// </summary>
	public enum ThermalSpecificHeatState
	{
		/// <summary>
		/// The curvature is not defined
		/// </summary>
		None = 0,

		/// <summary>
		/// The curvatire is defined by the code
		/// </summary>
		Code = 1,

		/// <summary>
		/// The curvature is input by the user
		/// </summary>
		User = 5,
	}

	/// <summary>
	/// Type of curvature - thermal stress-strain curvature
	/// </summary>
	public enum ThermalStressStrainState
	{
		/// <summary>
		/// The curvature is not defined
		/// </summary>
		None = 0,

		/// <summary>
		/// The curvatire is defined by the code
		/// </summary>
		Code = 1,

		/// <summary>
		/// The curvature is input by the user
		/// </summary>
		User = 5,
	}

	/// <summary>
	/// Type of curvature - thermal strain curvature
	/// </summary>
	public enum ThermalStrainState
	{
		/// <summary>
		/// The curvature is not defined
		/// </summary>
		None = 0,

		/// <summary>
		/// The curvatire is defined by the code
		/// </summary>
		Code = 1,

		/// <summary>
		/// The curvature is input by the user
		/// </summary>
		User = 5,
	}

	/// <summary>
	/// Material base class
	/// </summary>
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public abstract class Material : OpenElementId
	{
		/// <summary>
		/// Name of material
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Load from library - try override properties from library find material by name
		/// </summary>
		public bool LoadFromLibrary { get; set; }

		/// <summary>
		/// Young's modulus
		/// </summary>
		public double E { get; set; }

		/// <summary>
		/// Shear modulus
		/// </summary>
		public double G { get; set; }

		/// <summary>
		/// Poisson's ratio
		/// </summary>
		public double Poisson { get; set; }

		/// <summary>
		/// Unit weight
		/// </summary>
		public double UnitMass { get; set; }

		/// <summary>
		/// Specific heat capacity
		/// </summary>
		public double SpecificHeat { get; set; }

		/// <summary>
		/// Thermal expansion
		/// </summary>
		public double ThermalExpansion { get; set; }

		/// <summary>
		/// Thermal conductivity
		/// </summary>
		public double ThermalConductivity { get; set; }

		/// <summary>
		/// True if material is default material from the code
		/// </summary>
		public bool IsDefaultMaterial { get; set; }

		/// <summary>
		/// Order of this material in the code
		/// </summary>
		public int OrderInCode { get; set; }

		/// <summary>
		/// State of thermal expansion curvature
		/// </summary>
		public ThermalExpansionState StateOfThermalExpansion { get; set; }

		/// <summary>
		/// State of thermal conductivity curvature
		/// </summary>
		public ThermalConductivityState StateOfThermalConductivity { get; set; }

		/// <summary>
		/// State of thermal specific heat curvature
		/// </summary>
		public ThermalSpecificHeatState StateOfThermalSpecificHeat { get; set; }

		/// <summary>
		/// State of thermal specific stress-strain curvature
		/// </summary>
		public ThermalStressStrainState StateOfThermalStressStrain { get; set; }

		/// <summary>
		/// State of thermal strain curvature
		/// </summary>
		public ThermalStrainState StateOfThermalStrain { get; set; }

		/// <summary>
		/// User-defined curvature for thermal specific heat { x = Θ[K], y = c<sub>p</sub>[J/(kg K)] }
		/// </summary>
		public Polygon2D UserThermalSpecificHeatCurvature { get; set; }

		/// <summary>
		/// User-defined curvature for thermal conductivity curvature { x = Θ[K], y = λ<sub>c</sub>[W/(m K)] }
		/// </summary>
		public Polygon2D UserThermalConductivityCurvature { get; set; }

		/// <summary>
		/// User-defined curvature for thermal expansion curvature { x = Θ[K], y = γ[K<sup>-1</sup>] }
		/// </summary>
		public Polygon2D UserThermalExpansionCurvature { get; set; }

		/// <summary>
		/// User-defined curvature for thermal strain curvature { x = Θ[K], y = ε<sub>c</sub>[Δl/l] }
		/// </summary>
		public Polygon2D UserThermalStrainCurvature { get; set; }

		/// <summary>
		/// User-defined curvature for thermal stress,strain { Temperature = Θ[K], {x = ε[-], y = σ[Pa]}}
		/// </summary>
		public System.Collections.Generic.List<TemperatureCurve2D> UserThermalStressStrainCurvature { get; set; }
	}
}