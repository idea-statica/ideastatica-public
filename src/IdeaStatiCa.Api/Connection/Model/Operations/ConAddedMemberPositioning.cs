#nullable enable annotations

using IdeaRS.OpenModel.Geometry3D;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	public class ConAddedMemberPositioning
	{
		public ConAddedMemberPositioningEnum? PositioningType { get; set; }

		public double AngleAlpha { get; set; }

		public double AngleBeta { get; set; }

		public double RotationRx { get; set; }

		public Point3D? InsertPoint { get; set; }

		public Vector3D? Eccentricity { get; set; }

		public ConStiffeningMemberPlacement PlacementDefinition { get; set; }

		public Vector3D? AxisX { get; set; }

		public Vector3D? AxisY { get; set; }

		public Vector3D? AxisZ { get; set; }

		public int? OnMemberId { get; set; }

		public int? OnPlateIndex { get; set; }

		/// <summary>
		/// Optional id of an operation that produces the host plate. Used by
		/// <see cref="ConAddedMemberPositioningEnum.OnPlate"/> when the host plate is produced
		/// by a stiffening-plate / negative-plate operation rather than being part of the base members.
		/// Callers should set either <see cref="OnPlateIndex"/> (existing plate) or
		/// <see cref="OnPlateOperationId"/> (operation-produced plate), not both.
		/// </summary>
		public int? OnPlateOperationId { get; set; }

		/// <summary>
		/// Zero-based index into the host member's plates list (NOT a part-type discriminator).
		/// For a typical I-section: 0 = bottom flange, 1 = web, 2 = top flange. For Box sections
		/// the indices depend on the cross-section layout. The engine treats 0 as "unspecified"
		/// in some contexts — pick the actual plate the stiffener anchors on.
		/// </summary>
		public int OnMemberPlateIndex { get; set; }

		/// <summary>
		/// Zero-based index of the plate on the *added* member (the stiffener itself) that is
		/// welded against the host. Use 0 unless the added member has a multi-plate cross-section.
		/// </summary>
		public int AddedMemberPlateIndex { get; set; }

		public ConPlateFunction MountType { get; set; }

		public ConLocation MountLocation { get; set; }

		public int PlateEdgeIndex { get; set; }

		public double PositionX { get; set; }

		public double PositionY { get; set; }

		public double Rotation { get; set; }

		public double Pitch { get; set; }
	}
}
