using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Cement class
	/// </summary>
	[DataContract]
	public enum ConcCementClass
	{
		/// <summary>
		/// Cement of strength Classes CEM 32,5 N (Class S)
		/// </summary>
		S,

		/// <summary>
		/// Cement of strength Classes CEM 42,5 R, CEM 52,5 N and CEM 52,5 R (Class R)
		/// </summary>
		R,

		/// <summary>
		/// Cement of strength Classes CEM 32,5 R, CEM 42,5 N (Class N)
		/// </summary>
		N
	}

	/// <summary>
	/// Concrete aggreage type
	/// </summary>
	[DataContract]
	public enum ConcAggregateType
	{
		/// <summary>
		/// Quartzite aggregates
		/// </summary>
		Quartzite,

		/// <summary>
		///Limestone aggregates
		/// </summary>
		Limestone,

		/// <summary>
		/// Sandstone aggregates
		/// </summary>
		Sandstone,

		/// <summary>
		/// Basalt aggregates
		/// </summary>
		Basalt
	}

	/// <summary>
	/// Concrete stress-strain diagram for fire
	/// </summary>
	[DataContract]
	public enum ConcFireDiagramType
	{
		/// <summary>
		/// By the code with automatic temperature type
		/// </summary>
		ByCodeWithAutomaticTemperature,

		/// <summary>
		/// By the code with fixed temperature type
		/// </summary>
		ByCodeWithFixedTemperature
	}

	/// <summary>
	/// Concrete stress-strain diagram
	/// </summary>
	[DataContract]
	public enum ConcDiagramType
	{
		/// <summary>
		/// Bilinear type
		/// </summary>
		Bilinear,

		/// <summary>
		/// Parabolic type
		/// </summary>
		Parabolic,

		/// <summary>
		/// Defined by user
		/// </summary>
		DefinedByUser
	}

	/// <summary>
	/// Material concrete Ec2
	/// </summary>
	/// <example> 
	/// This sample shows how to create concrete material.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// //Cocrete material
	/// MatConcreteEc2 mat = new MatConcreteEc2();
	/// mat.Name = "Concrete1";
	/// mat.UnitMass = 2500.0;
	/// mat.E = 32.8e9;
	/// mat.G = 13.667e9;
	/// mat.Poisson = 0.2;
	/// mat.SpecificHeat = 0.6;
	/// mat.ThermalExpansion = 0.00001;
	/// mat.ThermalConductivity = 45;
	/// mat.Fck = 25.5e6;
	/// mat.CalculateDependentValues = true;
	/// 
	/// //Set s-s diagram as a default parabolic
	/// mat.DiagramType = ConcDiagramType.Parabolic;
	/// 
	/// //Set s-s diagram as an user defined
	/// mat.DiagramType = ConcDiagramType.DefinedByUser;
	/// var userDiagram = new Polygon2D();
	/// mat.UserDiagram = userDiagram;
	/// userDiagram.Points.Add(new Point2D() { X = -0.021, Y = 0 });
	/// userDiagram.Points.Add(new Point2D() { X = -0.02, Y = 0 });
	/// userDiagram.Points.Add(new Point2D() { X = -0.0025, Y = -80000000});
	/// userDiagram.Points.Add(new Point2D() { X = -0.0024, Y = -79868660.43});
	/// userDiagram.Points.Add(new Point2D() { X = -0.0023, Y = -79461961.9 });
	/// userDiagram.Points.Add(new Point2D() { X = -0.0022, Y = -78762709.44});
	/// userDiagram.Points.Add(new Point2D() { X = -0.0021, Y = -77756658.69});
	/// userDiagram.Points.Add(new Point2D() { X = -0.0019, Y = -74785483.74	});
	/// userDiagram.Points.Add(new Point2D() { X = -0.0017, Y = -70514061.33	});
	/// userDiagram.Points.Add(new Point2D() { X = -0.0015, Y = -64981949.46	});
	/// userDiagram.Points.Add(new Point2D() { X = -0.001	, Y = -46511627.91	});
	/// userDiagram.Points.Add(new Point2D() { X = -0.0005, Y = -23904382.47	});
	/// userDiagram.Points.Add(new Point2D() { X = 0, Y = 0});
	/// userDiagram.Points.Add(new Point2D() { X = 0.001, Y = 0 });
	/// 
	/// //Setting thermal characteristcs of material (in this case by the code)
	/// mat.StateOfThermalConductivity = ThermalConductivityState.Code;
	/// mat.StateOfThermalExpansion = ThermalExpansionState.Code;
	/// mat.StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code;
	/// mat.StateOfThermalStressStrain = ThermalStressStrainState.Code;
	/// 
	/// openModel.AddObject(mat);
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Libraries.Material.ECEN.MatConcreteECEN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatConcreteEc2 : MatConcrete
	{
		/// <summary>
		/// Setting calculation dependent values on f<sub>ck</sub>
		/// </summary>
		public bool CalculateDependentValues { get; set; }

		/// <summary>
		/// Characteristic compressive cylinder strength of concrete at 28 days - f<sub>ck</sub>
		/// </summary>
		public double Fck { get; set; }

		/// <summary>
		/// Secant modulus of elasticity of concrete - E<sub>cm</sub>
		/// </summary>
		public double Ecm { get; set; }

		/// <summary>
		/// Compressive strain in the concrete - ε<sub>c1</sub>
		/// </summary>
		public double Epsc1 { get; set; }

		/// <summary>
		/// Compressive strain in the concrete - ε<sub>c2</sub>
		/// </summary>
		public double Epsc2 { get; set; }

		/// <summary>
		/// Compressive strain in the concrete - ε<sub>c3</sub>
		/// </summary>
		public double Epsc3 { get; set; }

		/// <summary>
		/// Ultimate compressive strain in the concrete - ε<sub>cu1</sub>
		/// </summary>
		public double Epscu1 { get; set; }

		/// <summary>
		/// Ultimate compressive strain in the concrete - ε<sub>cu2</sub>
		/// </summary>
		public double Epscu2 { get; set; }

		/// <summary>
		/// Ultimate compressive strain in the concrete - ε<sub>cu3</sub>
		/// </summary>
		public double Epscu3 { get; set; }

		/// <summary>
		/// Mean value of axial tensile strength of concrete - f<sub>ctm</sub>
		/// </summary>
		public double Fctm { get; set; }

		/// <summary>
		/// Characteristic axial tensile strength of concrete 5% quantile - f<sub>ctk,0,05</sub>
		/// </summary>
		public double Fctk_0_05 { get; set; }

		/// <summary>
		/// Characteristic axial tensile strength of concrete 95% quantile - f<sub>ctk,0,95</sub>
		/// </summary>
		public double Fctk_0_95 { get; set; }

		/// <summary>
		/// Coefficient n-factor - necessary parabolic part of stress-strain diagram - n
		/// </summary>
		public double NFactor { get; set; }

		/// <summary>
		/// Mean value of concrete cylinder compressive strength - f<sub>cm</sub>
		/// </summary>
		public double Fcm { get; set; }

		/// <summary>
		/// Diameter of stone
		/// </summary>
		public double StoneDiameter { get; set; }

		/// <summary>
		/// Type of cement class
		/// </summary>
		public ConcCementClass CementClass { get; set; }

		/// <summary>
		/// Type of aggregate
		/// </summary>
		public ConcAggregateType AggregateType { get; set; }

		/// <summary>
		/// Type of stress-strain diagram bilinear/parabolic/general for ULS calculation
		/// </summary>
		public ConcDiagramType DiagramType { get; set; }

		///// <summary>
		///// Type of stress-strain diagram for fire ULS calculation
		///// </summary>
		//public ConcFireDiagramType FireDiagramType { get; set; }

		/// <summary>
		/// Contains silica fume
		/// </summary>
		public bool SilicaFume { get; set; }

		/// <summary>
		/// Stress strain diagram with tension part
		/// </summary>
		public bool PlainConcreteDiagram { get; set; }

		/// <summary>
		/// Stress-strain diagram defined by user
		/// </summary>
		public Polygon2D UserDiagram { get; set; }
	}
}