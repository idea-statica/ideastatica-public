using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Settlement of point support
	/// </summary>
	/// <example> 
	/// This sample shows how to create a .
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
	/// //Load case
	/// LoadCase loadCase = new LoadCase();
	/// //...
	/// openModel.AddObject(loadCase);
	/// 
	/// //Setlement on the Support A - 1mm in global Z - direction
	/// var settl = new Settlement();
	/// settl.ValueX = 0.0;
	/// settl.ValueY = 0.0;
	/// settl.ValueZ = -0.001;
	/// settl.ValueRx = 0.0;
	/// settl.ValueRy = 0.0;
	/// settl.ValueRz = 0.0;
	/// settl.Support = new ReferenceElement(pointSupportA);
	/// 
	/// openModel.AddObject(settl);
	/// loadCase.Settlements.Add(new ReferenceElement(settl));
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Loading.Settlement,CI.Loading", "CI.StructModel.Loading.ISettlement,CI.BasicTypes")]
	public class Settlement : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Settlement()
		{
		}

		/// <summary>
		/// Settlement in X dirrection of support
		/// </summary>
		public System.Double ValueX { get; set; }

		/// <summary>
		/// Settlement in Y dirrection of support
		/// </summary>
		public System.Double ValueY { get; set; }

		/// <summary>
		/// Settlement in Z dirrection of support
		/// </summary>
		public System.Double ValueZ { get; set; }

		/// <summary>
		/// Rotation around X-axis
		/// </summary>
		public System.Double ValueRx { get; set; }

		/// <summary>
		/// Rotation around Y-axis
		/// </summary>
		public System.Double ValueRy { get; set; }

		/// <summary>
		/// Rotation around Z-axis
		/// </summary>
		public System.Double ValueRz { get; set; }

		/// <summary>
		/// Support with settlement
		/// </summary>
		public ReferenceElement Support { get; set; }

	}
}
