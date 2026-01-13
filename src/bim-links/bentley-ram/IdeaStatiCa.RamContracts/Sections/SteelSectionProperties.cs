using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Sections
{
	public struct SteelSectionProperties
	{
		/// <summary>Section shape</summary>
		public ESTEEL_SEC Shape;

		/// <summary>Section label</summary>
		public string StrSize;

		/// <summary>Top Flange Width (breadth)</summary>
		public double BfTop;

		/// <summary>Bottom Flange Width (breadth)</summary>
		public double BFBot;

		/// <summary>Thickness of top flange</summary>
		public double TfTop;

		/// <summary>Thickness of bottom flange</summary>
		public double TFBot;

		/// <summary>K dimension top of section (refer to AISC for defn)</summary>
		public double kTop;

		/// <summary>K dimension bottom of section (refer to AISC for defn)</summary>
		public double kBot;

		/// <summary>Total depth of the section</summary>
		public double Depth;

		/// <summary>Thickness of the web of the section</summary>
		public double WebT;

		/// <summary>Warping torsion constant of section</summary>
		public double Cw;

		/// <summary>Torsional modulus of section</summary>
		public double J;

		/// <summary>True if this is a rolled section</summary>
		public ESTEEL_ROLLED_FLAG RolledFlag;

		/// <summary>Plastic modulus major axis</summary>
		public double Zx;

		/// <summary>Plastic modulus minor axis</summary>
		public double Zy;

		/// <summary>Section Modulus Top</summary>
		public double Sxtop;

		/// <summary>Section Modulus Bottom</summary>
		public double Sxbot;

		/// <summary>Section Modulus Minor Axis</summary>
		public double Sy;

		/// <summary>Major axis moment of inertia</summary>
		public double Imaj;

		/// <summary>Minor axis moment of inertia</summary>
		public double Imin;

		/// <summary>Area of cross section</summary>
		public double Area;
	}
}