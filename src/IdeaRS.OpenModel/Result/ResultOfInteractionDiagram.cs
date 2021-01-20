using IdeaRS.OpenModel.Geometry3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Point of interaction diagram
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class PointOfID
	{
		/// <summary>
		/// Normal force
		/// </summary>
		public double N { get; set; }

		/// <summary>
		/// Bending moment around y-axis
		/// </summary>
		public double My { get; set; }

		/// <summary>
		/// Bending moment around z-axis
		/// </summary>
		public double Mz { get; set; }
	}

	/// <summary>
	/// Polygon of pointIDs
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class PolygonPointID
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PolygonPointID()
		{
			Points = new List<PointOfID>();
		}

		/// <summary>
		/// Points
		/// </summary>
		public List<PointOfID> Points { get; set; }
	}

	///// <summary>
	///// Result of ID 
	///// </summary>
	//[Obfuscation(Feature = "renaming")]
	//public class ResultOfInteractionDiagram : SectionResultBase
	//{
	//	/// <summary>
	//	/// Constructor
	//	/// </summary>
	//	public ResultOfInteractionDiagram()
	//	{
	//		Polygons = new List<PolygonPointID>();
	//	}

	//	/// <summary>
	//	/// Planes
	//	/// </summary>
	//	public List<PolygonPointID> Polygons { get; set; }
	//}

	/// <summary>
	/// Result of Interaction diagram in plane
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class ResultOfInteractionDiagramPlane : SectionResultBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOfInteractionDiagramPlane()
		{
		}

		/// <summary>
		/// Polygon of interaction diagram plane
		/// </summary>
		public PolygonPointID Plane { get; set; }

		/// <summary>
		/// Polygon of Load
		/// </summary>
		public PolygonPointID LoadChart { get; set; }
	}

	///// <summary>
	///// Result of ID 2D
	///// </summary>
	//[Obfuscation(Feature = "renaming")]
	//public class ResultOfDisplacementChart : SectionResultBase
	//{
	//	/// <summary>
	//	/// Constructor
	//	/// </summary>
	//	public ResultOfDisplacementChart()
	//	{
	//	}

	//	/// <summary>
	//	/// Polygon of Load
	//	/// </summary>
	//	public PolygonPointID Load { get; set; }
	//}
}
