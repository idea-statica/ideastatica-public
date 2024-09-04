using System.Collections.Generic;

namespace IdeaRS.OpenModel.Connection
{
	public class FastenerGridBase : OpenElementId
	{
		/// <summary>
		/// Origin of the bolt grid LCS
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D Origin { get; set; }

		/// <summary>
		/// Bolt grid LCS - Axis X
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisX { get; set; }

		/// <summary>
		/// Bolt grid LCS - Axis Y
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisY { get; set; }

		/// <summary>
		/// Bolt grid LCS - Axis Z
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisZ { get; set; }

		/// <summary>
		/// Positions of holes in the local coordinate system of the grid
		/// </summary>
		public List<IdeaRS.OpenModel.Geometry3D.Point3D> Positions { get; set; }

		/// <summary>
		/// List of the connected parts
		/// </summary>
		public List<ReferenceElement> ConnectedParts { get; set; }

		/// <summary>
		/// Assembly
		/// </summary>
		public ReferenceElement Assembly { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Length
		/// </summary>
		public double Length
		{
			get;
			set;
		}
	}
}
