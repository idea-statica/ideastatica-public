namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Representation of element1D
	/// </summary>
	/// <example> 
	/// This sample shows how to create an element 1D .
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
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Structure.Element1D,CI.StructuralElements", "CI.StructModel.Structure.IElement1D,CI.BasicTypes")]
	public class Element1D : OpenElementId
	{
		/// <summary>
		/// Name of Element
		/// </summary>
		public System.String Name { get; set; }

		/// <summary>
		/// Gets, sets cross section located in the begining of this element1D
		/// </summary>
		public ReferenceElement CrossSectionBegin { get; set; }

		/// <summary>
		/// Gets, sets cross section located at the end of this element1D
		/// </summary>
		public ReferenceElement CrossSectionEnd { get; set; }

		/// <summary>
		/// Line segment with start point and end point
		/// </summary>
		[OpenModelProperty("LineSegment")]
		public ReferenceElement Segment { get; set; }

		/// <summary>
		/// Rotation of Cross-section of Element1D. Difference from default Line LCS
		/// </summary>
		public System.Double RotationRx { get; set; }

		/// <summary>
		/// Local eccentricity X at the begin of Element1D
		/// </summary>
		public System.Double EccentricityBeginX { get; set; }

		/// <summary>
		/// Local eccentricity Y at the begin of Element1D
		/// </summary>
		public System.Double EccentricityBeginY { get; set; }

		/// <summary>
		///  Local eccentricity Z at the begin of Element1D
		/// </summary>
		public System.Double EccentricityBeginZ { get; set; }

		/// <summary>
		///  Local eccentricity X at the end of Element1D
		/// </summary>
		public System.Double EccentricityEndX { get; set; }

		/// <summary>
		///  Local eccentricity Y at the end of Element1D
		/// </summary>
		public System.Double EccentricityEndY { get; set; }

		/// <summary>
		///  Local eccentricity Z at the end of Element1D
		/// </summary>
		public System.Double EccentricityEndZ { get; set; }
	}
}