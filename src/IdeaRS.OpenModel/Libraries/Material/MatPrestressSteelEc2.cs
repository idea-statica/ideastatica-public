using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{

	/// <summary>
	/// Type of prestress steel
	/// </summary>
	[DataContract]
	public enum PrestressSteelType
	{
		/// <summary>
		/// Plain round wire - hladké  dráty
		/// </summary>
		PlainRoundWire,

		/// <summary>
		/// Indneted wire - draty s vtisky
		/// </summary>
		IndentedWire,

		/// <summary>
		/// Strand - lana
		/// </summary>
		Strand,

		/// <summary>
		/// Plain round bar - hladké tyče 
		/// </summary>
		PlainRoundBar,

		/// <summary>
		/// Ribbed bar - žebírkové tyče
		/// </summary>
		RibbedBar,

		/// <summary>
		/// Compacted strand - lana
		/// </summary>
		CompactedStrand
	}

	/// <summary>
	/// Surface characteristic
	/// </summary>
	[DataContract]
	public enum SurfaceCharacteristicType
	{
		/// <summary>
		/// Plain
		/// </summary>
		Plain,

		/// <summary>
		/// Indented
		/// </summary>
		Indented,

		/// <summary>
		/// Ribbed
		/// </summary>
		Ribbed
	}

	/// <summary>
	/// Production type
	/// </summary>
	[DataContract]
	public enum ProductionType
	{
		/// <summary>
		/// Hot rolled and processed
		/// </summary>
		HotRolledAndProcessed,

		/// <summary>
		/// Patented
		/// </summary>
		Patented,

		/// <summary>
		/// Cold drawn
		/// </summary>
		ColdDrawn,

		/// <summary>
		/// Stress relieved
		/// </summary>
		StressRelieved,

		/// <summary>
		/// Low relaxation
		/// </summary>
		LowRelaxation
	}

	/// <summary>
	/// Material prestressing steel Ec2
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.ECEN.MatPrestressSteelECEN,CI.Material", "CI.StructModel.Libraries.Material.IMatPrestressSteel,CI.BasicTypes", typeof(MatPrestressSteel))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatPrestressSteelEc2 : MatPrestressSteel
	{

		/// <summary>
		/// Constructor
		/// </summary>
		public MatPrestressSteelEc2()
		{
		}

		/// <summary>
		/// Char. value of max. force (prEN 10138)
		/// </summary>
		public double Fm
		{
			get;
			set;
		}

		/// <summary>
		/// Char. 0.1% proof force
		/// </summary>
		public double Fp01
		{
			get;
			set;
		}

		/// <summary>
		/// Total elongation at max. force (prEN 10138)
		/// </summary>
		public double Agt
		{
			get;
			set;
		}

		/// <summary>
		/// Fatigue stress range (prEN 10138)
		/// </summary>
		public double Fr
		{
			get;
			set;
		}

		/// <summary>
		/// Characteristic tensile strength (EN 1992-1-1)
		/// </summary>
		public double Fpk
		{
			get;
			set;
		}

		/// <summary>
		/// 0,1% proof-stress (EN 1992-1-1)
		/// </summary>
		public double Fp01k
		{
			get;
			set;
		}

		/// <summary>
		/// Characteristic strain at maximum load (EN 1992-1-1)
		/// </summary>
		public double Epsuk
		{
			get;
			set;
		}

		/// <summary>
		/// Type of prestress steel
		/// </summary>
		public PrestressSteelType Type
		{
			get;
			set;
		}

		/// <summary>
		/// Surface characteristic
		/// </summary>
		public SurfaceCharacteristicType SurfaceCharacteristic
		{
			get;
			set;
		}

		/// <summary>
		/// Production type
		/// </summary>
		public ProductionType Production
		{
			get;
			set;
		}

		/// <summary>
		/// Type of diagram
		/// </summary>
		public ReinfDiagramType DiagramType
		{
			get;
			set;
		}
	}
}
