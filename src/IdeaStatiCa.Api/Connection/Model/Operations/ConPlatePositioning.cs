#nullable enable annotations

using IdeaRS.OpenModel.Geometry3D;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	public class ConPlatePositioning
	{
		public ConPlatePositioningEnum? ConPlatePositioningType { get; set; }

		public int ConcreteBlockIndex { get; set; }

		public int ConcreteSurface { get; set; }

		public double PositionX { get; set; }

		public double PositionY { get; set; }

		public double RotationAngle { get; set; }
		
		public ConLocalCoordinateSystem? CoordinateSystem { get; set; }
		
		public Point3D? Offset { get; set; }

		public Point3D? Rotation { get; set; }
		
		public int OnMemberId { get; set; }

		/// <summary>
		/// Optional id of an operation that produces the host member (stiffening / negative-volume / stub member).
		/// Used by <see cref="ConPlatePositioningEnum.MemberLcs"/> and <see cref="ConPlatePositioningEnum.Member"/>
		/// instead of <see cref="OnMemberId"/> when the host is an operation-produced member. Callers should
		/// set either <see cref="OnMemberId"/> (real beam) or <see cref="OnMemberOperationId"/> (operation host), not both.
		/// </summary>
		public int? OnMemberOperationId { get; set; }

		public ConWorkPlaneMethod InputMethod { get; set; }

		public Vector3D? NormalVector { get; set; }
		
		public int PlateOnMemberIndex { get; set; }

		public ConPlateFunction Function { get; set; }

		public ConLocation Location { get; set; }

		public double Pitch { get; set; }

		public int PlateIndex { get; set; }

		public int? OperationId { get; set; }

		public int PlateEdgeIndex { get; set; }
	}
}
