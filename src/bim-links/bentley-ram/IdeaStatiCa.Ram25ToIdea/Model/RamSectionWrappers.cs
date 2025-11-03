using RamDataAcc1 = RAMDATAACCESSLib.RamDataAccess1;
using IModel = RAMDATAACCESSLib.IModel;
using RamDataAccIDBIO = RAMDATAACCESSLib.IDBIO1;
using IMemberData = RAMDATAACCESSLib.IMemberData1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea
{
	internal class RamSectionWrapper
	{
		//Use member to access a SectionWrapper
		//int IMemberId;//IMemberID, Member’s Unique ID
		internal RAMDATAACCESSLib.ESTEEL_SEC SteelSectionShape = RAMDATAACCESSLib.ESTEEL_SEC.EStlNone; //peShape, Section shape
		internal RAMDATAACCESSLib.ESTEEL_ROLLED_FLAG RolledFlag = RAMDATAACCESSLib.ESTEEL_ROLLED_FLAG.EStlRolled; //peRolledFlag, True if this is a rolled section

		internal string SectionLabel = ""; //pstrSize, Section label

		public double Depth; //pdDepth, Total depth of the section
		public double Tw; //pdWebT, Thickness of the web of the section,
		public double TfTop; //pdTfTop, Thickness of top flange
		public double TfBot; //pdTFBot, Thickness of bottom flange
		public double BfTop; //pdBfTop, Top Flange Width(breadth)
		public double BfBot; //pdBFBot, Bottom Flange Width(breadth)

		public int MaterialId;

		//Other Avaliable Section Properties
		//pdkTop, K dimension top of section(refer to AISC for defn)
		//pdkBot ,K dimension bottom of section(refer to AISC for defn)
		//pdCw, Warping torsion constant of section
		//pdJ, Torsional modulus of section
		//pdZx, Plastic modulus major axis
		//pdZy, Plastic modulus minor axis
		//pdSxtop, Section Modulus Top
		//pdSxbot, Section Modulus Bottom
		//pdSy, Section Modulus Minor Axis
		//pdImaj, Major axis moment of inertia
		//pdImin, Minor axis moment of inertia
		//padArea, Area of cross section

		public RamSectionWrapper() { }
	}
}
