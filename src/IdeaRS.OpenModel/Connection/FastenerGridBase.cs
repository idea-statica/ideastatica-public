using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	[DataContract]
	public class FastenerGridBase : OpenElementId
	{
		/// <summary>
		/// Origin of the bolt grid LCS
		/// </summary>
		[DataMember]
		public IdeaRS.OpenModel.Geometry3D.Point3D Origin { get; set; }

		/// <summary>
		/// Bolt grid LCS - Axis X
		/// </summary>
		[DataMember]
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisX { get; set; }

		/// <summary>
		/// Bolt grid LCS - Axis Y
		/// </summary>
		[DataMember]
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisY { get; set; }

		/// <summary>
		/// Bolt grid LCS - Axis Z
		/// </summary>
		[DataMember]
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisZ { get; set; }

		/// <summary>
		/// Positions of holes in the local coordinate system of the grid
		/// </summary>
		[DataMember]
		public List<IdeaRS.OpenModel.Geometry3D.Point3D> Positions { get; set; }

		/// <summary>
		/// List of the connected parts
		/// </summary>
		[DataMember]
		public List<ReferenceElement> ConnectedParts { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Length
		/// </summary>
		[DataMember]
		public double Length
		{
			get;
			set;
		}
	}
}
