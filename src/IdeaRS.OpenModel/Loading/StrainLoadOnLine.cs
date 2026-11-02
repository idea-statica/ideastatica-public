namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Linearly distributed generalized strain load along a line.
	/// Strain load is in local coordinate system and there are no possible eccentricities.
	/// </summary>
	/// <example> 
	/// This sample shows how to create a strain load on the line.
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
	/// //Load case
	/// LoadCase loadCase = new LoadCase();
	/// //...
	/// openModel.AddObject(loadCase);
	/// 
	/// 	/Uniform strain load on line1 - relative between 0.5-1.0. epsX = -0.0002
	/// StrainLoadOnLine strainLoadLine = new StrainLoadOnLine();
	/// strainLoadLine.Bimp = new StrainImpulse() { EpsX = -2e-4 };
	/// strainLoadLine.Eimp = new StrainImpulse() { EpsX = -2e-4 };
	/// strainLoadLine.GeometryPolyLine = new ReferenceElement(line1);
	/// strainLoadLine.RelativeBeginPosition = 0.5;
	/// strainLoadLine.RelativeBeginPosition = 1.0;
	/// 
	/// openModel.AddObject(strainLoadLine);
	/// loadCase.StrainLoadsOnLine.Add(new ReferenceElement(strainLoadLine));
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Loading.StrainLoadOnLine,CI.Loading", "CI.StructModel.Loading.IStrainLoadOnLine,CI.BasicTypes")]
	public class StrainLoadOnLine : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public StrainLoadOnLine()
		{
		}

		/// <summary>
		/// Gets or sets the strain impulse at the begin position.
		/// </summary>
		public StrainImpulse Bimp { get; set; }

		/// <summary>
		/// Gets or sets the strain impulse at the end position.
		/// </summary>
		public StrainImpulse Eimp { get; set; }

		/// <summary>
		/// PolyLine3D or Segment3D
		/// </summary>
		[OpenModelProperty("GeometryPolyLine")]
		public ReferenceElement Geometry { get; set; }

		/// <summary>
		/// Beginnig position on ISegment3D
		/// </summary>
		public System.Double RelativeBeginPosition { get; set; }

		/// <summary>
		/// End position on ISegment3D
		/// </summary>
		public System.Double RelativeEndPosition { get; set; }
	}

	/// <summary>
	/// Generalized strain load impulse.
	/// </summary>
	public struct StrainImpulse
	{
		/// <summary>
		/// The normal strain.
		/// </summary>
		public System.Double EpsX { get; set; }

		/// <summary>
		/// The torsion slope around x axis.
		/// </summary>
		public System.Double PhiX { get; set; }

		/// <summary>
		/// The curvature around z axis.
		/// </summary>
		public System.Double PhiY { get; set; }

		/// <summary>
		/// The curvature around y axis.
		/// </summary>
		public System.Double PhiZ { get; set; }

		/// <summary>
		/// The shear strain.
		/// </summary>
		public System.Double GammaXY { get; set; }

		/// <summary>
		/// The shear strain.
		/// </summary>
		public System.Double GammaXZ { get; set; }
	}
}