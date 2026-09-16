#nullable enable annotations

using IdeaRS.OpenModel.Geometry3D;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Work plane operation. A work plane is virtual reference geometry used by other operations
	/// (e.g. cuts) and fatigue analysis. It has no structural effect by itself.
	/// </summary>
	public class ConWorkPlaneOperation : ConOperation
	{
		public ConWorkPlaneOperation() : base()
		{
			Active = true;
		}

		[JsonConstructor]
		public ConWorkPlaneOperation(int id) : base(id)
		{
			Active = true;
		}

		/// <summary>
		/// How the work plane is defined (by angles, normal vector, or intersection of members).
		/// </summary>
		public ConWorkPlaneMethod Method { get; set; }

		/// <summary>
		/// Coordinate system the work plane is related to (joint, member, or plate).
		/// </summary>
		public ConWorkPlaneRelatedTo RelatedTo { get; set; }

		/// <summary>
		/// Origin of the work plane in the chosen coordinate system.
		/// Null treated as zero origin.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Point3D? Origin { get; set; }

		/// <summary>
		/// Normal vector of the work plane (used when <see cref="Method"/> is <see cref="ConWorkPlaneMethod.ByNormalVector"/>).
		/// Null treated as unit Z.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Vector3D? NormalVector { get; set; }

		/// <summary>
		/// Rotation around X axis in radians (used when <see cref="Method"/> is <see cref="ConWorkPlaneMethod.ByAngles"/>).
		/// </summary>
		public double RotationX { get; set; }

		/// <summary>
		/// Rotation around Y axis in radians (used when <see cref="Method"/> is <see cref="ConWorkPlaneMethod.ByAngles"/>).
		/// </summary>
		public double RotationY { get; set; }

		/// <summary>
		/// Rotation around Z axis in radians (used when <see cref="Method"/> is <see cref="ConWorkPlaneMethod.ByAngles"/>).
		/// </summary>
		public double RotationZ { get; set; }

		/// <summary>
		/// If true the work plane defines a fatigue section.
		/// </summary>
		public bool IsFatigue { get; set; }

		/// <summary>
		/// Optional member reference (required only when <see cref="RelatedTo"/> is <see cref="ConWorkPlaneRelatedTo.Member"/>
		/// or <see cref="Method"/> is <see cref="ConWorkPlaneMethod.ByIntersection"/>).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MemberId { get; set; }

		/// <summary>
		/// Optional plate reference (required only when <see cref="RelatedTo"/> is <see cref="ConWorkPlaneRelatedTo.Plate"/>).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? PlateId { get; set; }

		/// <summary>
		/// Optional id of the operation that produces the host plate (stiffening-plate, negative-plate, ...).
		/// Set this instead of <see cref="PlateId"/> when the work plane is anchored on an operation-produced plate.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? PlateOperationId { get; set; }

		/// <summary>
		/// Optional second member reference, used only when <see cref="Method"/> is <see cref="ConWorkPlaneMethod.ByIntersection"/>.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? RelatedMemberId { get; set; }

		/// <summary>
		/// Offset along X from the intersection point (used only when <see cref="Method"/> is <see cref="ConWorkPlaneMethod.ByIntersection"/>).
		/// </summary>
		public double OffsetX { get; set; }

		/// <summary>
		/// If true the near intersection is used, otherwise the far one (used only when <see cref="Method"/> is <see cref="ConWorkPlaneMethod.ByIntersection"/>).
		/// </summary>
		public bool NearIntersection { get; set; }
	}
}
