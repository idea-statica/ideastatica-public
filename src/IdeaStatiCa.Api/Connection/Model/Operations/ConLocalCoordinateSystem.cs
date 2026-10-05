#nullable enable annotations

using IdeaRS.OpenModel.Geometry3D;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	public class ConLocalCoordinateSystem
	{
		public ConLocalCoordinateSystem(Point3D origin, Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
		{
			Origin = origin;
			XAxis = xAxis;
			YAxis = yAxis;
			ZAxis = zAxis;
		}

		/// <summary>
		/// Origin point for LCS
		/// </summary>
		public Point3D Origin { get; set; }

		/// <summary>
		/// Axis X unit vector
		/// </summary>
		public Vector3D XAxis { get; set; }

		/// <summary>
		/// Axis Y unit vector
		/// </summary>
		public Vector3D YAxis { get; set; }

		/// <summary>
		/// Axis Z unit vector
		/// </summary>
		public Vector3D ZAxis { get; set; }
	}
}
