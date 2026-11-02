using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Cement class
	/// </summary>
	public enum ConcCementClassSIA
	{
		/// <summary>
		/// cement of strength Classes CEM 32,5 N (Class S)
		/// </summary>
		S,

		/// <summary>
		/// cement of strength Classes CEM 42,5 R, CEM 52,5 N and CEM 52,5 R (Class R)
		/// </summary>
		R,

		/// <summary>
		/// cement of strength Classes CEM 32,5 R, CEM 42,5 N (Class N)
		/// </summary>
		N
	}

	/// <summary>
	/// Concrete aggreage type
	/// </summary>
	public enum ConcAggregateTypeSIA
	{
		/// <summary>
		/// Alluvial gravel
		/// </summary>
		AlluvialGravel,

		/// <summary>
		/// Crushed limestone
		/// </summary>
		CrushedLimestone,

		/// <summary>
		/// Micaceous
		/// </summary>
		Micaceous
	}

	/// <summary>
	/// crack width requirement
	/// </summary>
	public enum CrackWidthRequirementsSIA
	{
		/// <summary>
		/// calculation is nod defined it means not necessary
		/// </summary>
		NotNecessary,

		/// <summary>
		/// type A
		/// </summary>
		A,

		/// <summary>
		/// type B
		/// </summary>
		B,

		/// <summary>
		/// type C
		/// </summary>
		C,

		/// <summary>
		/// value is define as fsd-value
		/// </summary>
		Value
	}

	/// <summary>
	/// Material concrete SIA
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.SIA.MatConcreteSIA,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatConcreteSIA : MatConcrete
	{
		/// <summary>
		/// setting calculation dependent values on fck
		/// </summary>
		public bool CalculateDependentValues { get; set; }

		/// <summary>
		/// characteristic compressive cylinder strength of concrete at 28 days
		/// </summary>
		public double Fck { get; set; }

		/// <summary>
		/// mean value of concrete cylinder compressive strength
		/// </summary>
		public double Fcm { get; set; }

		/// <summary>
		/// secant modulus of elasticity of concrete
		/// </summary>
		public double Ecm { get; set; }

		/// <summary>
		/// mean value of axial tensile strength of concrete
		/// </summary>
		public double Fctm { get; set; }

		/// <summary>
		/// characteristic axial tensile strength of concrete 5% quantile
		/// </summary>
		public double Fctk_0_05 { get; set; }

		/// <summary>
		/// characteristic axial tensile strength of concrete 95% quantile
		/// </summary>
		public double Fctk_0_95 { get; set; }

		/// <summary>
		/// conversion factor nfc
		/// </summary>
		public double Nfc { get; set; }

		/// <summary>
		/// characteristic  value of the shear stress limit
		/// </summary>
		public double Tck { get; set; }

		/// <summary>
		/// compressive strain in the concrete
		/// </summary>
		public double Epsc1d { get; set; }

		/// <summary>
		/// compressive strain in the concrete
		/// </summary>
		public double Epsc2d { get; set; }

		/// <summary>
		/// diameter of stone
		/// </summary>
		public double StoneDiameter { get; set; }

		/// <summary>
		/// type of aggregate
		/// </summary>
		public ConcAggregateTypeSIA AggregateType { get; set; }

		/// <summary>
		/// type of cement class
		/// </summary>
		public ConcCementClassSIA CementClass { get; set; }

		/// <summary>
		/// type of stress-strain diagram bilinear/parabolic/general for ULS calculation
		/// </summary>
		public ConcDiagramType DiagramType { get; set; }

		/// <summary>
		/// Stress strain diagram with tension part
		/// </summary>
		public bool PlainConcreteDiagram { get; set; }
		
	}
}
