using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Initial deformation of point - geometry nonlinear calculation
	/// </summary>
	/// <example> 
	/// This sample shows how to create an initial imperfection of the point.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	///
	/// //Create nodes
	/// Point3D pointA = new Point3D();
	/// pointA.X = 0;
	/// pointA.Y = 0;
	/// pointA.Z = 0;
	/// openModel.AddObject(pointA);
	/// 
	/// Point3D pointB = new Point3D();
	/// pointB.X = 0;
	/// pointB.Y = 0;
	/// pointB.Z = 1.2;
	/// openModel.AddObject(pointB);
	/// 
	/// Point3D pointC = new Point3D();
	/// pointC.X = 0;
	/// pointC.Y = 0;
	/// pointC.Z = 2.4;
	/// openModel.AddObject(pointC);
	/// 
	/// //Line between nodes
	/// LineSegment3D line1 = new LineSegment3D();
	/// line1.StartPoint = new ReferenceElement(pointA);
	/// line1.EndPoint = new ReferenceElement(pointB);
	/// //LCS of line
	/// line1.LocalCoordinateSystem = new CoordSystemByZup();
	/// openModel.AddObject(line1);
	/// 
	/// //Line between nodes
	/// LineSegment3D line2 = new LineSegment3D();
	/// line2.StartPoint = new ReferenceElement(pointB);
	/// line2.EndPoint = new ReferenceElement(pointC);
	/// //LCS of line
	/// line2.LocalCoordinateSystem = new CoordSystemByZup();
	/// openModel.AddObject(line2);
	/// 
	/// //Cocrete material
	/// MatConcreteEc2 mat = new MatConcreteEc2();
	/// //...
	/// openModel.AddObject(mat);
	/// 
	/// //Reinforcement material
	/// MatReinforcementEc2 matR = new MatReinforcementEc2();
	/// //...
	/// openModel.AddObject(matR);
	/// 
	/// //Cross-section without the bars
	/// IdeaRS.OpenModel.CrossSection.CrossSectionComponent css = new IdeaRS.OpenModel.CrossSection.CrossSectionComponent();
	/// //...
	/// openModel.AddObject(css);
	/// 
	/// //Reinforced cross-section (referemnces cross-section and ads the bars)
	/// ReinforcedCrossSection rcs = new ReinforcedCrossSection();
	/// //...
	/// openModel.AddObject(rcs);
	/// 
	/// //Model element along the "line1" - part of member1D
	/// Element1D element1D1 = new Element1D();
	/// element1D1.CrossSectionBegin = new ReferenceElement(rcs);
	/// element1D1.CrossSectionEnd = new ReferenceElement(rcs);
	/// element1D1.Segment = new ReferenceElement(line1);
	/// openModel.AddObject(element1D1);
	/// 
	/// //Model element along the "line2" - part of member1D
	/// Element1D element1D2 = new Element1D();
	/// element1D2.CrossSectionBegin = new ReferenceElement(rcs);
	/// element1D2.CrossSectionEnd = new ReferenceElement(rcs);
	/// element1D2.Segment = new ReferenceElement(line2);
	/// openModel.AddObject(element1D2);
	/// 
	/// //Model member assembles element1D
	/// Member1D member1D = new Member1D();
	/// member1D.Elements1D.Add(new ReferenceElement(element1D1));
	/// member1D.Elements1D.Add(new ReferenceElement(element1D2));
	/// member1D.Member1DType = Member1DType.Column;
	/// member1D.Name = "COLUMN1";
	/// openModel.AddObject(member1D);
	/// 
	/// //Point support in the bottom node
	/// PointSupportNode pointSupportA = new PointSupportNode();
	/// pointSupportA.Point = new ReferenceElement(pointA);
	/// pointSupportA.SupportTypeRX = SupportTypeInDirrection.Free;
	/// pointSupportA.SupportTypeRY = SupportTypeInDirrection.Free;
	/// pointSupportA.SupportTypeRZ = SupportTypeInDirrection.Rigid;
	/// pointSupportA.SupportTypeX = SupportTypeInDirrection.Rigid;
	/// pointSupportA.SupportTypeY = SupportTypeInDirrection.Rigid;
	/// pointSupportA.SupportTypeZ = SupportTypeInDirrection.Rigid;
	/// openModel.AddObject(pointSupportA);
	/// 
	/// //Point support in the top node
	/// PointSupportNode pointSupportB = new PointSupportNode();
	/// pointSupportB.Point = new ReferenceElement(pointC);
	/// pointSupportB.SupportTypeRX = SupportTypeInDirrection.Free;
	/// pointSupportB.SupportTypeRY = SupportTypeInDirrection.Free;
	/// pointSupportB.SupportTypeRZ = SupportTypeInDirrection.Free;
	/// pointSupportB.SupportTypeX = SupportTypeInDirrection.Rigid;
	/// pointSupportB.SupportTypeY = SupportTypeInDirrection.Rigid;
	/// pointSupportB.SupportTypeZ = SupportTypeInDirrection.Free;
	/// openModel.AddObject(pointSupportB);
	///
	/// //set initial imperfection to node B and C
	/// InitialImperfectionOfPoint initPointB = new InitialImperfectionOfPoint();
	/// initPointB.Displacement = new Vector3D();
	/// initPointB.Displacement.X = 0.005;
	/// initPointB.Point = new ReferenceElement(pointB);
	/// openModel.AddObject(initPointB);
	/// 
	/// InitialImperfectionOfPoint initPointC = new InitialImperfectionOfPoint();
	/// initPointC.Displacement = new Vector3D();
	/// initPointC.Displacement.X = 0.01;
	/// initPointC.Point = new ReferenceElement(pointC);
	/// openModel.AddObject(initPointC);
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Structure.InitialImperfectionOfPoint,CI.StructuralElements", "CI.StructModel.Structure.IInitialImperfectionOfPoint,CI.BasicTypes")]
	public class InitialImperfectionOfPoint : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public InitialImperfectionOfPoint()
		{
			Displacement = new Vector3D() { X = 0, Y = 0, Z = 0 };
		}

		/// <summary>
		/// Reference to geometrical point
		/// </summary>
		public ReferenceElement Point { get; set; }

		/// <summary>
		/// Displacement of structure point
		/// </summary>
		public Vector3D Displacement { get; set; }
	}
}