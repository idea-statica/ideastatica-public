using IdeaRS.OpenModel.Geometry2D;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Steel stress-strain diagram
	/// </summary>
	[DataContract]
	public enum SteelDiagramType
	{
		/// <summary>
		/// Billinear
		/// </summary>
		[DataMember]
		Bilinear = 0,

		/// <summary>
		/// Defined by user
		/// </summary>
		[DataMember]
		DefinedByUser = 5
	}

	/// <summary>
	/// Material steel Ec2
	/// </summary>
	/// <example> 
	/// This sample shows how to create steel material.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// //Steel material
	/// MatSteelEc2 matS = new MatSteelEc2();
	/// matS.Name = "S275";
	/// matS.UnitMass = 7850.0;
	/// matS.E = 200e9;
	/// matS.Poisson = 0.2;
	/// matS.G = 83.333e9;
	/// matS.SpecificHeat = 0.6;
	/// matS.ThermalExpansion = 0.00001;
	/// matS.ThermalConductivity = 45;
	/// matS.fy = 235e6;
	/// matS.fu = 360e6;
	/// matS.fy40 = 215e6;
	/// matS.fu40 = 340e6;
	/// 
	/// //Setting thermal characteristcs of material (in this case by the code)
	/// matS.StateOfThermalConductivity = ThermalConductivityState.Code;
	/// matS.StateOfThermalExpansion = ThermalExpansionState.Code;
	/// matS.StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code;
	/// matS.StateOfThermalStressStrain = ThermalStressStrainState.Code;
	/// 
	/// matS.DiagramType = SteelDiagramType.Bilinear;
	/// openModel.AddObject(matS);
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Libraries.Material.ECEN.MatSteelECEN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	[DataContract]
	public class MatSteelEc2 : MatSteel
	{
		/// <summary>
		/// Yield strength for nominal thickness of the element &lt;= 40mm - f<sub>y</sub>
		/// </summary>
		[DataMember]
		public double fy { get; set; }

		/// <summary>
		/// Ultimate strength  for nominal thickness of the element &lt;= 40mm - f<sub>u</sub>
		/// </summary>
		[DataMember]
		public double fu { get; set; }

		/// <summary>
		/// Yield strength for nominal thickness of the element &gt; 40mm and &lt;= 100mm - f<sub>y,(&gt;40)</sub>
		/// </summary>
		[DataMember]
		public double fy40 { get; set; }

		/// <summary>
		/// Ultimate strength for nominal thickness of the element &gt; 40mm and &lt;= 100mm - f<sub>u,(&gt;40)</sub>
		/// </summary>
		[DataMember]
		public double fu40 { get; set; }

		/// <summary>
		/// Type of material diagram
		/// </summary>
		[DataMember]
		public SteelDiagramType DiagramType { get; set; }

		/// <summary>
		/// Stress-strain diagram defined by user
		/// </summary>
		[DataMember]
		public Polygon2D UserDiagram { get; set; }

		/// <summary>
		/// Material strength for specific thickness of plate
		/// </summary>
		[DataMember]
		public MaterialStrengthProperty MaterialStrength { get; set; }
	}
}