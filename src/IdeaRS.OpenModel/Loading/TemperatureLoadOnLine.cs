using IdeaRS.OpenModel.Geometry2D;
using System.Collections.Generic;

namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Temperature direction
	/// </summary>
	public enum TemperatureDirection
	{
		/// <summary>
		/// Every where
		/// </summary>
		EveryWhere,

		/// <summary>
		/// Specified edges
		/// </summary>
		SpecifiedEdges
	}

	/// <summary>
	/// Cross-section component edges
	/// </summary>
	[OpenModelClass("CI.StructModel.Loading.CrossSectionComponentEdges,CI.Loading")]
	public class CrossSectionComponentEdges : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CrossSectionComponentEdges()
		{
			EdgeIndexes = new List<int>();
		}

		/// <summary>
		/// Zero based index of component
		/// </summary>
		public int ComponentIndex { get; set; }

		/// <summary>
		/// List of zero based edge indexes of component
		/// </summary>
		public List<int> EdgeIndexes { get; set; }
	}

	/// <summary>
	/// Temperature load on line
	/// </summary>
	/// <example> 
	/// This sample shows how to create a temperature load.
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
	/// //temperature load on the line 2 defined to edge 0 and 1 of the component 0 of crossection
	/// var tempload = new TemperatureLoadOnLine();
	/// tempload.Geometry = new ReferenceElement(line2);
	/// tempload.RelativeBeginPosition = 0.0;
	/// tempload.RelativeEndPosition = 1.0;
	/// tempload.Direction = TemperatureDirection.SpecifiedEdges;
	/// var edges = new CrossSectionComponentEdges();
	/// edges.ComponentIndex = 0;
	/// edges.EdgeIndexes.Add(0);
	/// edges.EdgeIndexes.Add(1);
	/// tempload.DirectionEdges.Add(edges);
	/// tempload.ConvectionCoefficient = 25.0;
	/// tempload.RadiationCoefficient = 0.0;
	/// 
	/// //temperature curve sec vs K
	/// var curve = new Polygon2D();
	/// curve.Points.Add(new Point2D() { X = 0, Y = 296.15 });
	/// curve.Points.Add(new Point2D() { X = 600, Y = 954.15 });
	/// curve.Points.Add(new Point2D() { X = 1200, Y = 1057.15 });
	/// curve.Points.Add(new Point2D() { X = 1800, Y = 1117.15 });
	/// curve.Points.Add(new Point2D() { X = 2400, Y = 1160.15 });
	/// curve.Points.Add(new Point2D() { X = 3000, Y = 1194.15 });
	/// curve.Points.Add(new Point2D() { X = 3600, Y = 1221.15 });
	/// curve.Points.Add(new Point2D() { X = 4200, Y = 1244.15 });
	/// curve.Points.Add(new Point2D() { X = 4800, Y = 1264.15 });
	/// curve.Points.Add(new Point2D() { X = 5200, Y = 1276.15 });
	/// curve.Points.Add(new Point2D() { X = 6000, Y = 1296.15 });
	/// tempload.TemperatureCurve = curve;
	/// 
	/// openModel.AddObject(tempload);
	/// loadCase.TemperatureLoadsOnLine.Add(new ReferenceElement(tempload));
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Loading.TemperatureLoadOnLine,CI.Loading", "CI.StructModel.Loading.ITemperatureLoadOnLine,CI.BasicTypes")]
	public class TemperatureLoadOnLine : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public TemperatureLoadOnLine()
		{
			DirectionEdges = new List<CrossSectionComponentEdges>();
		}

		/// <summary>
		/// Temperature curve  { x = t[s], y = Θ[K] }
		/// </summary>
		public Polygon2D TemperatureCurve { get; set; }

		/// <summary>
		/// Segment3D
		/// </summary>
		public ReferenceElement Geometry { get; set; }

		/// <summary>
		/// Relative position on segment - begin
		/// </summary>
		public double RelativeBeginPosition { get; set; }

		/// <summary>
		/// Relative position on segment - end
		/// </summary>
		public double RelativeEndPosition { get; set; }

		/// <summary>
		/// Direction on temperaure
		/// </summary>
		public TemperatureDirection Direction { get; set; }

		/// <summary>
		/// Edges of direction
		/// </summary>
		public List<CrossSectionComponentEdges> DirectionEdges { get; set; }

		/// <summary>
		/// Convection coefficient per area - α<sub>c</sub>[W/(m<sup>2</sup>K)]
		/// </summary>
		public double ConvectionCoefficient { get; set; }

		/// <summary>
		/// Radiation coefficient per area - α<sub>r</sub>[W/(m<sup>2</sup>K)]
		/// </summary>
		public double RadiationCoefficient { get; set; }
	}
}