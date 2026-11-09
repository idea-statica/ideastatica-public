using System;
using System.Collections.Generic;
using System.Linq;

namespace CadExampleApi
{
	/// <summary>
	/// Reads the model of the example CAD application. In a real link, this is the API of your application.
	/// Lengths are in metres.
	/// </summary>
	public interface ICadGeometryApi
	{
		/// <summary>
		/// Asks the user for the point to design a connection at. Returns null when the user cancels.
		/// </summary>
		CadPoint3D PickConnectionPoint();

		/// <summary>
		/// The members the user has selected.
		/// </summary>
		IEnumerable<CadMember> GetSelectedMembers();

		/// <summary>
		/// The plates, bolt grids and cuts the user has selected.
		/// </summary>
		IEnumerable<CadObject> GetSelectedConnectionObjects();

		/// <summary>
		/// Every member in the model.
		/// </summary>
		IEnumerable<CadMember> GetAllMembers();

		/// <summary>
		/// Every object in the model: members, plates, bolt grids and cuts.
		/// </summary>
		IEnumerable<CadObject> GetAllObjects();

		/// <summary>
		/// The member with the given id.
		/// </summary>
		CadMember GetMember(int id);

		/// <summary>
		/// The member's own axes, with the origin at its start. X runs from the start of the member to its end. Y is
		/// horizontal, or global Y for a vertical member.
		/// </summary>
		CadCoordinateSystem GetMemberCoordinateSystem(int id);

		/// <summary>
		/// The plate with the given id. It can be a negative plate.
		/// </summary>
		CadPlate GetPlate(int id);

		/// <summary>
		/// The bolt grid with the given id.
		/// </summary>
		CadBoltGrid GetBoltGrid(int id);

		/// <summary>
		/// The cut with the given id.
		/// </summary>
		CadCut GetCut(int id);

		/// <summary>
		/// The cuts that shape the given member or plate.
		/// </summary>
		IEnumerable<CadCut> GetCutsOf(int partId);

		/// <summary>
		/// The cross-section with the given name.
		/// </summary>
		CadCrossSection GetCrossSection(string name);

		/// <summary>
		/// The material with the given name.
		/// </summary>
		CadMaterial GetMaterial(string name);
	}

	/// <summary>
	/// Serves a fixed model: a beam bolted to a column through an end plate, with an opening in the beam web.
	/// The user is assumed to have selected everything.
	/// </summary>
	public class CadGeometryApi : ICadGeometryApi
	{
		private readonly List<CadMaterial> _materials = new List<CadMaterial>();
		private readonly List<CadCrossSection> _crossSections = new List<CadCrossSection>();
		private readonly List<CadMember> _members = new List<CadMember>();
		private readonly List<CadPlate> _plates = new List<CadPlate>();
		private readonly List<CadBoltGrid> _boltGrids = new List<CadBoltGrid>();
		private readonly List<CadCut> _cuts = new List<CadCut>();

		public CadGeometryApi()
		{
			CreateModel();
		}

		public CadPoint3D PickConnectionPoint() => new CadPoint3D(-2.0, 3.0, 3.0);

		public IEnumerable<CadMember> GetSelectedMembers() => _members;

		public IEnumerable<CadObject> GetSelectedConnectionObjects()
			=> _plates.Where(plate => !plate.IsNegative).Cast<CadObject>().Concat(_boltGrids).Concat(_cuts);

		public IEnumerable<CadMember> GetAllMembers() => _members;

		public IEnumerable<CadObject> GetAllObjects()
			=> _members.Cast<CadObject>().Concat(_plates).Concat(_boltGrids).Concat(_cuts);

		public CadMember GetMember(int id) => _members.Single(member => member.Id == id);

		public CadCoordinateSystem GetMemberCoordinateSystem(int id)
		{
			CadMember member = GetMember(id);
			CadVector3D x = member.Start.VectorTo(member.End).Normalize();
			CadVector3D globalZ = new CadVector3D(0, 0, 1);

			bool isVertical = Math.Abs(Math.Abs(x.Z) - 1) < 1e-9;
			CadVector3D y = isVertical
				? new CadVector3D(0, 1, 0)
				: globalZ.Cross(x).Normalize();

			return new CadCoordinateSystem(member.Start, x, y, x.Cross(y));
		}

		public CadPlate GetPlate(int id) => _plates.Single(plate => plate.Id == id);

		public CadBoltGrid GetBoltGrid(int id) => _boltGrids.Single(boltGrid => boltGrid.Id == id);

		public CadCut GetCut(int id) => _cuts.Single(cut => cut.Id == id);

		public IEnumerable<CadCut> GetCutsOf(int partId) => _cuts.Where(cut => cut.CutPart.Id == partId);

		public CadCrossSection GetCrossSection(string name) => _crossSections.Single(section => section.Name == name);

		public CadMaterial GetMaterial(string name) => _materials.Single(material => material.Name == name);

		/// <summary>
		/// Builds the fixed model described on the class.
		/// </summary>
		private void CreateModel()
		{
			_materials.Add(new CadMaterial("S 355", CadMaterialType.Steel));
			_materials.Add(new CadMaterial("8.8", CadMaterialType.BoltGrade));

			_crossSections.Add(new CadCrossSection("HE240B", "S 355", 0.240, 0.240));
			_crossSections.Add(new CadCrossSection("HE200B", "S 355", 0.200, 0.200));

			var column = new CadMember(1, new CadPoint3D(-2.0, 3.0, 0.0), new CadPoint3D(-2.0, 3.0, 6.0), "HE240B");
			var beam = new CadMember(2, new CadPoint3D(-2.0, 3.0, 3.0), new CadPoint3D(2.0, 3.0, 3.0), "HE200B");
			_members.Add(column);
			_members.Add(beam);

			// The end plate stands on the column flange; the beam is cut back to it.
			var endPlatePlane = new CadCoordinateSystem(
				new CadPoint3D(-1.87, 2.88, 2.7),
				new CadVector3D(0, 1, 0),
				new CadVector3D(0, 0, 1),
				new CadVector3D(1, 0, 0));
			var endPlate = new CadPlate(3, Rectangle(endPlatePlane, 0.0, 0.0, 0.24, 0.5), "S 355", 0.020, isNegative: false);
			_plates.Add(endPlate);
			_cuts.Add(new CadCut(4, beam, endPlate, isSurfaceCut: false));

			// A square opening in the beam web, cut by a negative plate.
			var openingPlane = new CadCoordinateSystem(
				new CadPoint3D(-1.75, 3.0, 3.0),
				new CadVector3D(0, 0, 1),
				new CadVector3D(-1, 0, 0),
				new CadVector3D(0, -1, 0));
			var opening = new CadPlate(5, Rectangle(openingPlane, -0.05, -0.05, 0.05, 0.05), "S 355", 0.050, isNegative: true);
			_plates.Add(opening);
			_cuts.Add(new CadCut(6, beam, opening, isSurfaceCut: true));

			var m16 = new CadBoltAssembly
			{
				Name = "M16 8.8",
				GradeName = "8.8",
				Diameter = 0.016,
				HoleDiameter = 0.018,
				HeadDiameter = 0.024,
				HeadDiagonalDiameter = 0.026,
				HeadHeight = 0.010,
				NutThickness = 0.013,
				GrossArea = 0.000201,
				TensileStressArea = 0.000157,
			};
			var boltPositions = new List<CadPoint2D>
			{
				new CadPoint2D(0.05, 0.05),
				new CadPoint2D(0.05, 0.45),
				new CadPoint2D(0.19, 0.05),
				new CadPoint2D(0.19, 0.45),
			};
			_boltGrids.Add(new CadBoltGrid(7, m16, endPlatePlane, boltPositions, new List<ICadPart> { column, endPlate }));
		}

		/// <summary>
		/// A closed rectangle in the XY plane of the coordinate system.
		/// </summary>
		private static CadOutline2D Rectangle(CadCoordinateSystem plane, double minX, double minY, double maxX, double maxY)
		{
			return new CadOutline2D(plane, new List<CadPoint2D>
			{
				new CadPoint2D(minX, minY),
				new CadPoint2D(maxX, minY),
				new CadPoint2D(maxX, maxY),
				new CadPoint2D(minX, maxY),
				new CadPoint2D(minX, minY),
			});
		}
	}
}
