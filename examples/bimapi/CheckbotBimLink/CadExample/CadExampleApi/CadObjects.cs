using System.Collections.Generic;

namespace CadExampleApi
{
	/// <summary>
	/// The kinds of material in the CAD library.
	/// </summary>
	public enum CadMaterialType
	{
		Steel,
		BoltGrade,
	}

	/// <summary>
	/// A material from the CAD library. Model objects refer to it by name.
	/// </summary>
	public class CadMaterial
	{
		public CadMaterial(string name, CadMaterialType type)
		{
			Name = name;
			Type = type;
		}

		public string Name { get; }

		public CadMaterialType Type { get; }
	}

	/// <summary>
	/// A cross-section from the CAD library. Members refer to it by name.
	/// </summary>
	public class CadCrossSection
	{
		public CadCrossSection(string name, string materialName, double width, double height)
		{
			Name = name;
			MaterialName = materialName;
			Width = width;
			Height = height;
		}

		public string Name { get; }

		public string MaterialName { get; }

		public double Width { get; }

		public double Height { get; }
	}

	/// <summary>
	/// A bolt type from the CAD library, with the dimensions IDEA StatiCa needs. Lengths are in metres, areas in
	/// square metres.
	/// </summary>
	public class CadBoltAssembly
	{
		public string Name { get; set; }

		public string GradeName { get; set; }

		public double Diameter { get; set; }

		public double HoleDiameter { get; set; }

		public double HeadDiameter { get; set; }

		public double HeadDiagonalDiameter { get; set; }

		public double HeadHeight { get; set; }

		public double NutThickness { get; set; }

		public double GrossArea { get; set; }

		public double TensileStressArea { get; set; }
	}

	/// <summary>
	/// An object placed in the CAD model.
	/// </summary>
	public abstract class CadObject
	{
		protected CadObject(int id)
		{
			Id = id;
		}

		public int Id { get; }
	}

	/// <summary>
	/// A member or a plate: something bolts can hold and a cut can shape.
	/// </summary>
	public interface ICadPart
	{
		int Id { get; }
	}

	/// <summary>
	/// A straight member between two points, with a cross-section from the CAD library.
	/// </summary>
	public class CadMember : CadObject, ICadPart
	{
		public CadMember(int id, CadPoint3D start, CadPoint3D end, string crossSectionName)
			: base(id)
		{
			Start = start;
			End = end;
			CrossSectionName = crossSectionName;
		}

		public CadPoint3D Start { get; }

		public CadPoint3D End { get; }

		public string CrossSectionName { get; }
	}

	/// <summary>
	/// A plate. A negative plate is not a part of the structure: it is a body that cuts an opening into a part.
	/// </summary>
	public class CadPlate : CadObject, ICadPart
	{
		public CadPlate(int id, CadOutline2D outline, string materialName, double thickness, bool isNegative)
			: base(id)
		{
			Outline = outline;
			MaterialName = materialName;
			Thickness = thickness;
			IsNegative = isNegative;
		}

		public CadOutline2D Outline { get; }

		public string MaterialName { get; }

		public double Thickness { get; }

		public bool IsNegative { get; }
	}

	/// <summary>
	/// Bolts of one assembly at positions in the XY plane of a coordinate system, holding parts together.
	/// </summary>
	public class CadBoltGrid : CadObject
	{
		public CadBoltGrid(
			int id,
			CadBoltAssembly boltAssembly,
			CadCoordinateSystem coordinateSystem,
			IReadOnlyList<CadPoint2D> positions,
			IReadOnlyList<ICadPart> connectedParts)
			: base(id)
		{
			BoltAssembly = boltAssembly;
			CoordinateSystem = coordinateSystem;
			Positions = positions;
			ConnectedParts = connectedParts;
		}

		public CadBoltAssembly BoltAssembly { get; }

		public CadCoordinateSystem CoordinateSystem { get; }

		public IReadOnlyList<CadPoint2D> Positions { get; }

		public IReadOnlyList<ICadPart> ConnectedParts { get; }
	}

	/// <summary>
	/// A part cut by another part.
	/// </summary>
	public class CadCut : CadObject
	{
		public CadCut(int id, ICadPart cutPart, ICadPart cuttingPart, bool isSurfaceCut)
			: base(id)
		{
			CutPart = cutPart;
			CuttingPart = cuttingPart;
			IsSurfaceCut = isSurfaceCut;
		}

		public ICadPart CutPart { get; }

		public ICadPart CuttingPart { get; }

		/// <summary>
		/// True when the cut follows the surface of the cutting part, false when it follows its bounding box.
		/// </summary>
		public bool IsSurfaceCut { get; }
	}
}
