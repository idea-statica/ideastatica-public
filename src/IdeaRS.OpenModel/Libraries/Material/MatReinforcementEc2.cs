using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Reinforcement class type
	/// </summary>
	[DataContract]
	public enum ReinfClass
	{
		/// <summary>
		/// Class A
		/// </summary>
		A,

		/// <summary>
		/// Class B
		/// </summary>
		B,

		/// <summary>
		/// Class C
		/// </summary>
		C
	}

	/// <summary>
	/// Type of reinforcement
	/// </summary>
	[DataContract]
	public enum ReinfType
	{
		/// <summary>
		/// Bars
		/// </summary>
		Bars,

		/// <summary>
		/// Decoiled rods
		/// </summary>
		DecoiledRods,

		/// <summary>
		/// Wire fabrics
		/// </summary>
		WireFabrics,

		/// <summary>
		/// Lattice girders
		/// </summary>
		LatticeGirders
	}

	/// <summary>
	/// Reinforcement fabrication
	/// </summary>
	[DataContract]
	public enum ReinfFabrication
	{
		/// <summary>
		/// Hot rolled
		/// </summary>
		HotRolled,

		/// <summary>
		/// Cold worked
		/// </summary>
		ColdWorked
	}

	/// <summary>
	/// Stress-strain reeinforcemnt diagram for fire
	/// </summary>
	[DataContract]
	public enum ReinfFireDiagramType
	{
		/// <summary>
		/// Billinear with an inclined top branch
		/// </summary>
		FireBilinerWithAnInclinedTopBranch,

		/// <summary>
		/// Billinear without an inclined top branch
		/// </summary>
		FireBilinerWithOutAnInclinedTopBranch
	}

	/// <summary>
	/// Material reinforcement Ec2
	/// </summary>
	/// <example> 
	/// This sample shows how to create reinforcement material.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// //Reinforcement material
	/// MatReinforcementEc2 matR = new MatReinforcementEc2();
	/// matR.Name = "Reinf";
	/// matR.UnitMass = 7850.0;
	/// matR.E = 200e9;
	/// matR.Poisson = 0.2;
	/// matR.G = 83.333e9;
	/// matR.SpecificHeat = 0.6;
	/// matR.ThermalExpansion = 0.00001;
	/// matR.ThermalConductivity = 45;
	/// matR.BarSurface = ReinfBarSurface.Ribbed;
	/// matR.Fyk = 387e6;
	/// matR.CoeffFtkByFyk = 1.05;
	/// matR.Epsuk = 0.025;
	/// matR.Class = ReinfClass.A;
	/// matR.Type = ReinfType.Bars;
	/// matR.Fabrication = ReinfFabrication.HotRolled;
	/// matR.DiagramType = ReinfDiagramType.BilinerWithAnInclinedTopBranch;
	/// 
	/// //Setting thermal characteristcs of material (in this case by the code)
	/// matR.StateOfThermalConductivity = ThermalConductivityState.Code;
	/// matR.StateOfThermalExpansion = ThermalExpansionState.Code;
	/// matR.StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code;
	/// matR.StateOfThermalStressStrain = ThermalStressStrainState.Code;
	/// 
	/// openModel.AddObject(matR);
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Libraries.Material.ECEN.MatReinforcementECEN,CI.Material", "CI.StructModel.Libraries.Material.IMatReinforcement,CI.BasicTypes", typeof(MatReinforcement))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatReinforcementEc2 : MatReinforcement
	{
		/// <summary>
		/// Characteristic yield strength of reinforcement - f<sub>yk</sub>
		/// </summary>
		public double Fyk { get; set; }

		/// <summary>
		/// Coefficient  f<sub>tk</sub>/f<sub>yk</sub> - k
		/// </summary>
		public double CoeffFtkByFyk { get; set; }

		/// <summary>
		/// Characteristic strain of reinforcement at maximum load - ε<sub>uk</sub>
		/// </summary>
		public double Epsuk { get; set; }

		/// <summary>
		/// Characteristic tensile strength of reinforcement - f<sub>tk</sub>
		/// </summary>
		public double Ftk { get; set; }

		/// <summary>
		/// Class of reinforcement
		/// </summary>
		public ReinfClass Class { get; set; }

		/// <summary>
		/// Type of reinforcement
		/// </summary>
		public ReinfType Type { get; set; }

		/// <summary>
		/// Fabrication of reinforcement
		/// </summary>
		public ReinfFabrication Fabrication { get; set; }

		/// <summary>
		/// Type of material diagram
		/// </summary>
		public ReinfDiagramType DiagramType { get; set; }

		///// <summary>
		///// Type of material diagram for fire resistance
		///// </summary>
		//public ReinfFireDiagramType FireDiagramType { get; set; }

		/// <summary>
		/// Stress-strain diagram defined by user
		/// </summary>
		public Polygon2D UserDiagram { get; set; }
	}
}