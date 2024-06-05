using System.Collections.Generic;
using System;
using System.Drawing;

namespace BimApiLinkCadExample.CadExampleApi
{

	//Cad Object Can be Extended as Required for your 
	public abstract class CadObject
	{
		public int Id { get; }

		public CadObject(int id)
		{
			Id = id;
		}

	}

	public class CadMaterial : CadObject
	{
		public string Name { get; set; }

		public CadMaterial(string materialName, int id) : base(id)
		{
			Name = materialName;
		}
	}

	public class CadCrossSection : CadObject
	{
		public string Name { get; set; }

		public string MaterialId { get; set; }

		public double CrossSectionWidth { get; }

		public double CrossSectionHeight { get; }

		public CadCrossSection(string sectionName, string material, double crossSectionWidth, double crossSectionHeight, int id) : base(id)
		{
			Name = sectionName;
			MaterialId = material;
			CrossSectionWidth = crossSectionWidth;
			CrossSectionHeight = crossSectionHeight;
		}
	}

	public class CadMember : CadObject, IConnectedPart
	{
		public CadPoint3D StartPoint { get; set; }
		public CadPoint3D EndPoint { get; set; }
		public string CrossSection { get; set; }

		public CadMember(CadPoint3D startPt, CadPoint3D endPt, string crossSection, int id) : base(id)
		{
			StartPoint = startPt;
			EndPoint = endPt;
			CrossSection = crossSection;
		}

		public string PartType { get { return this.GetType().Name; } }

		public int PartId { get { return this.Id; } }
	}

	public interface IConnectedPart
	{
		string PartType { get;}

		int PartId { get; }
	}

	public class CadPlate : CadObject, IConnectedPart
	{
		public CadOutline2D CadOutline2D { get; set; }

		public string Material { get; set; }

		public double Thickness { get; set; }

		public bool IsNegativeObject { get; set; }

		public string PartType { get { return this.GetType().Name; } }
		
		public int PartId { get { return this.Id; } }

		public CadPlate(CadOutline2D geom, string material, double thickness, int id) : base(id)
		{
			CadOutline2D = geom;
			Material = material;
			Thickness = thickness;
		}
	}

	public class CadBoltGrid : CadObject
	{
		public string BoltGrade { get; set; }

		public string BoltAssembly { get; set; }

		public double BoltLength { get; set; }

		public CadPlane3D BoltPlane { get; set; }

		public List<CadPoint2D> BoltPositions { get; set; }

		public List<IConnectedPart> ConnectedParts { get; set; }

		public CadBoltGrid(string boltGrade, string boltAssembly, CadPlane3D plane, List<CadPoint2D> positions, List<IConnectedPart> connectedParts, int id) : base(id)
		{
			BoltGrade = boltGrade;
			BoltAssembly = boltAssembly;
			BoltPlane = plane;
			BoltPositions = positions;
			ConnectedParts = connectedParts;
		}
	}

	public class CadCutByPart : CadObject
	{
		public IConnectedPart PartToCut { get; set; }
		public IConnectedPart CuttingPart { get; set; }

		//Provide additional properties of different avaliable cuts to drive the IDEA StatiCa cut settings. 
		public bool IsMemberYAxisContourCut { get; set; } = false;

		public CadCutByPart(IConnectedPart partToCut, IConnectedPart cutter, int id) : base(id)
		{
			PartToCut = partToCut;
			CuttingPart = cutter;
		}
	}

	//Class not used in the Example
	public class CadWeld : CadObject
	{
		public enum WeldType { Butt, Fillet, Double_Fillet }
		
		public double Size { get; set; }

		public string Material { get; set; }

		public CadPoint3D Start { get; set; }

		public CadPoint3D End { get; set; }

		public List<IConnectedPart> ConnectedParts { get; set; }

		public CadWeld(double size, string material, CadPoint3D start, CadPoint3D end, List<IConnectedPart> connectedParts, int id) : base(id)
		{
			Size = size;
			Material = material;
			Start = start;
			End = end;
			ConnectedParts = connectedParts;
		}
	}
}
