using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Type of prestress steel
	/// </summary>
	public enum PrestressSteelTypeSIA
	{
		/// <summary>
		/// wire - drát
		/// </summary>
		Wire,

		/// <summary>
		/// Strand - lana
		/// </summary>
		Strand,

		/// <summary>
		/// bar - tyč
		/// </summary>
		Bar
	}

	/// <summary>
	/// Surface characteristic
	/// </summary>
	public enum SurfaceCharacteristicTypeSIA
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
	/// Material prestressing steel SIA
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.SIA.MatPrestressSteelSIA,CI.Material", "CI.StructModel.Libraries.Material.IMatPrestressSteel,CI.BasicTypes", typeof(MatPrestressSteel))]
	public class MatPrestressSteelSIA : MatPrestressSteel
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatPrestressSteelSIA()
		{
		}

		/// <summary>
		/// characteristic value of tensile strength of prestressing steel
		/// </summary>
		public double Fpk
		{
			get;
			set;
		}

		/// <summary>
		/// characteristic value of yield strength of prestressing steel
		/// </summary>
		public double Fp01k
		{
			get;
			set;
		}

		/// <summary>
		/// characteristic value of ultimate strain of reinforcing steel or strain at maximum load for prestressing steel
		/// </summary>
		public double Epsuk
		{
			get;
			set;
		}

		/// <summary>
		/// dimensioning value of yield strength of prestressing steel
		/// </summary>
		public double Fpd
		{
			get;
			set;
		}

		/// <summary>
		/// dimensioning value of ultimate strain of reinforcing steel or prestressing steel
		/// </summary>
		public double Epsud
		{
			get;
			set;
		}

		/// <summary>
		/// Type of prestress steel
		/// </summary>
		public PrestressSteelTypeSIA Type
		{
			get;
			set;
		}

		/// <summary>
		/// Surface characteristic
		/// </summary>
		public SurfaceCharacteristicTypeSIA SurfaceCharacteristic
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
