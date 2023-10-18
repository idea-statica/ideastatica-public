using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using System.Collections.Generic;
using System.Linq;

namespace BimApiLinkCadExample.CadExampleApi
{
	public interface ICadGeometryApi
	{
		CadMember GetMember(int id);

		(UnitVector3D X, UnitVector3D Y, UnitVector3D Z) GetMemberLcs(int id);

		IEnumerable<CadMember> GetAllMembers();

		//Returns identifiers for the currently selected members in the model
		IEnumerable<CadMember> GetSelectedMembers();

		//Returns identifiers of Connection objects which are currently selected in the model
		IEnumerable<CadObject> GetSelectPartObjects();

		//This Api Method would allow prompting to the user to select a point in the model which should be used as the connection centre,
		//This would be called on single connection import.
		CadPoint3D GetConnectionPoint();

		IEnumerable<CadWeld> GetAllWelds();

		IEnumerable<CadCutByPart> GetAllCuts();

		IEnumerable<CadBoltGrid> GetAllBoltGrids();

		CadPlate GetPlate(int id);

		CadBoltGrid GetBoltGrid(int id);

		CadWeld GetWeld(int id);

		CadCutByPart GetCutByPart(int id);

		IEnumerable<CadPlate> GetAllPlates();

		CadCrossSection GetCrossSection(int id);
		
		CadCrossSection GetCrossSectionByName(string name);

		CadMaterial GetMaterial(int id);

		CadMaterial GetMaterialByName(string name);

		CadObject GetObjectById(int id);

	}

	public class CadGeometryApi : ICadGeometryApi
	{
		private List<CadMember> _members = new List<CadMember>();
		private List<CadPlate> _plates = new List<CadPlate>();
		private List<CadWeld> _welds = new List<CadWeld>();
		private List<CadBoltGrid> _boltGrids = new List<CadBoltGrid>();
		private List<CadCutByPart> _cutByPart = new List<CadCutByPart>();
		
		
		private List<CadMaterial> _materials = new List<CadMaterial>();
		private List<CadCrossSection> _crossSections = new List<CadCrossSection>();

		public CadGeometryApi() 
		{
			//Initializes the Test Geometry
			//Replace 'Get' methods here with API methods to Geometry objects from your Application.

			InitilizeTestGeometry(); 
		}

		public CadMember GetMember(int id) => _members.FirstOrDefault(m => m.Id == id);

		public CadMaterial GetMaterial(int id) => _materials.FirstOrDefault(m => m.Id == id);

		public CadMaterial GetMaterialByName(string name) => _materials.FirstOrDefault(m => m.Name == name);

		public CadCrossSection GetCrossSection(int  id) => _crossSections.FirstOrDefault(m => m.Id == id);

		public CadCrossSection GetCrossSectionByName(string name) => _crossSections.FirstOrDefault(m => m.Name == name);

		public (UnitVector3D X, UnitVector3D Y, UnitVector3D Z) GetMemberLcs(int id)
		{
			var member = GetMember(id);
			
			CadPoint3D start = member.StartPoint;
			CadPoint3D end = member.EndPoint;
			
			return CalculateMemberLcs(start, end);
		}

		public IEnumerable<CadMember> GetAllMembers() => _members;

		private static (UnitVector3D X, UnitVector3D Y, UnitVector3D Z) CalculateMemberLcs(CadPoint3D begin, CadPoint3D end)
		{
			//Converting CadPoints to Math.Net Points to use Vector Math

			var beginPt = new Point3D(begin.X, begin.Y, begin.Z);
			var endPt = new Point3D(end.X, end.Y, end.Z); 

			Vector3D memberX = endPt - beginPt;
			
			UnitVector3D globalZ = UnitVector3D.ZAxis;

			UnitVector3D memberY;

			if (memberX.IsParallelTo(globalZ))
			{
				// column
				memberY = UnitVector3D.YAxis;
			}
			else
			{
				// beam
				memberY = memberX.CrossProduct(globalZ).Normalize().Negate();
			}

			return (memberX.Normalize(), memberY, memberX.CrossProduct(memberY).Normalize());
		}


		//DUMBY CAD GEOMETRY DATA
		private void InitilizeTestGeometry()
		{
			int id = 1;

			//ADDING MATERIALS
			_materials.Add(new CadMaterial("S 355", id++));
			_materials.Add(new CadMaterial("S500", id++));

			//ADDING CROSS-SECTIONS
			_crossSections.Add(new CadCrossSection("HE240B", "S 355", id++));
			_crossSections.Add(new CadCrossSection("HE200B", "S 355", id++));

			//ADDING MEMBERS
			CadMember column = new CadMember(new CadPoint3D(-2.0, 3.0, 0), new CadPoint3D(-2, 3, 6), "HE240B", id++);
			_members.Add(column);
			CadMember beam = new CadMember(new CadPoint3D(-2.0, 3.0, 3), new CadPoint3D(2, 3, 3), "HE200B", id++);
			_members.Add(beam);

			//ADDING PLATES
			CadPlane3D plane = new CadPlane3D(new CadPoint3D(-1.87, 2.88, 2.7),
				new CadVector3D(0, 1, 0),
				new CadVector3D(0, 0, 1),
				new CadVector3D(1, 0, 0));

			List<CadPoint2D> polyLine = new List<CadPoint2D>
			{
				new CadPoint2D(0, 0),
				new CadPoint2D(0.24, 0),
				new CadPoint2D(0.24, 0.5),
				new CadPoint2D(0, 0.5),
				new CadPoint2D(0, 0)
			};
			CadOutline2D outline = new CadOutline2D(plane, polyLine);
			CadPlate plate = new CadPlate(outline, "S 355", 0.020, id++);

			_plates.Add(plate);

			//ADDING BOLT GRIDS
			var positions = new List<CadPoint2D>() {
				new CadPoint2D(0.05, 0.05),
				new CadPoint2D(0.05, 0.45),
				new CadPoint2D(0.19, 0.05),
				new CadPoint2D(0.19, 0.45)};

			var connectedItems = new List<IConnectedPart>() { column, plate };
			_boltGrids.Add(new CadBoltGrid("S500", "M16", plane, positions, connectedItems, id++));

			//ADD CUTS
			_cutByPart.Add(new CadCutByPart(beam, plate, id++));

		}

		public IEnumerable<CadMember> GetSelectedMembers()
		{
			//In the Dumby API we will automaticaly provide all the Id's of the populated Dumby Geometry
			return GetAllMembers();
		}

		public IEnumerable<CadObject> GetSelectPartObjects()
		{
			//In the Dumby API we will automaticaly provide all the Id's of the populated Dumby conneciton Items
			var connectionItems = new List<CadObject>();

			connectionItems.AddRange(_plates);
			connectionItems.AddRange(_boltGrids);
			connectionItems.AddRange(_cutByPart);

			return connectionItems;
		}

		public IEnumerable<CadWeld> GetAllWelds() => _welds;

		public IEnumerable<CadBoltGrid> GetAllBoltGrids() => _boltGrids;

		public IEnumerable<CadCutByPart> GetAllCuts() => _cutByPart;

		public IEnumerable<CadPlate> GetAllPlates() => _plates;

		public CadPoint3D GetConnectionPoint()
		{
			return new CadPoint3D(-2.0, 3.0, 3);
		}

		public CadPlate GetPlate(int id) => _plates.FirstOrDefault(m => m.Id == id);

		public CadBoltGrid GetBoltGrid(int id) => _boltGrids.FirstOrDefault(m => m.Id == id);

		public CadWeld GetWeld(int id) => _welds.FirstOrDefault(m => m.Id == id);

		public CadCutByPart GetCutByPart(int id) => _cutByPart.FirstOrDefault(m => m.Id == id);

		public CadObject GetObjectById(int id)
		{
			throw new System.NotImplementedException();
		}
	}
}