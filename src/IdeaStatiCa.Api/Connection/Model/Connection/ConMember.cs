using System;
using IdeaRS.OpenModel.Geometry3D;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConMember : ConItem
	{
		public ConMember() : base()
		{
			IsBearing = false;
		}

		[JsonConstructor]
		public ConMember(int id, bool isBearing) : base(id)
		{
			IsBearing = isBearing;
		}

		public bool IsContinuous { get; set; }

		public int? CrossSectionId { get; set; }

		/// <summary>
		/// Which end of the member is connected in the joint.
		/// Combined with <see cref="ConMemberPosition.AxisX"/>, this disambiguates the two halves
		/// of a through-column that share the same axis but join the connection from opposite sides.
		/// </summary>
		public ConMemberConnectedByEnum ConnectedBy { get; set; }

		/// <summary>
		/// Normalised position of the joint along the member's reference line:
		/// <c>0</c> = begin, <c>1</c> = end.
		/// </summary>
		/// <remarks>
		/// Back-compatibility shim over <see cref="ConnectedBy"/>. Only the two endpoints are
		/// representable now (<c>Begin</c>/<c>End</c>): a set value of <c>0.5</c> or any value
		/// below <c>0.5</c> maps to <see cref="ConMemberConnectedByEnum.Begin"/>, so the former
		/// through-member (<c>0.5</c>) case no longer round-trips. Not serialized;
		/// <see cref="ConnectedBy"/> is the wire contract.
		/// </remarks>
		[Obsolete("Use " + nameof(ConnectedBy) + " instead. PositionOnRefLine will be removed in a future version.")]
		[JsonIgnore]
		public double PositionOnRefLine
		{
			get => ConnectedBy == ConMemberConnectedByEnum.End ? 1.0 : 0.0;
			set => ConnectedBy = value >= 0.5 ? ConMemberConnectedByEnum.End : ConMemberConnectedByEnum.Begin;
		}

		public bool? MirrorY { get; set; }

		public bool? MirrorZ { get; set; }

		public bool IsBearing { get; }

		public ConMemberPosition Position { get; set; }

		public ConMemberModel Model { get; set; }

		public ConStiffnessAnalysis StiffnessAnalysis { get; set; }
	}

	public class ConMemberModel
	{
		public ConMemberModelTypeEnum ModelTypeEnum { get; set; }

		/// <summary>
		/// Analysis model type as a string.
		/// </summary>
		/// <remarks>
		/// Back-compatibility shim over <see cref="ModelTypeEnum"/>. The getter returns the enum
		/// member name; the setter parses by name (case-insensitive) and ignores unrecognised
		/// values, leaving <see cref="ModelTypeEnum"/> unchanged. Not serialized;
		/// <see cref="ModelTypeEnum"/> is the wire contract.
		/// </remarks>
		[Obsolete("Use " + nameof(ModelTypeEnum) + " instead. ModelType (string) will be removed in a future version.")]
		[JsonIgnore]
		public string ModelType
		{
			get => ModelTypeEnum.ToString();
			set
			{
				if (Enum.TryParse<ConMemberModelTypeEnum>(value, ignoreCase: true, out var parsed))
				{
					ModelTypeEnum = parsed;
				}
			}
		}

		public ConMemberForcesInEnum ForcesIn { get; set; }

		// X for position
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public double? X { get; set; }

		// for Connected member face
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? ConnectedMemberId { get; set; }
	}

	public class ConAlignedPlate : ConMemberPlate
	{
		public ConAlignedPlateSideCodeEnum PlateSide { get; set; }
	}

	public class ConMemberPlate
	{
		public int MemberId { get; set; }

		public ConMemberPlatePartTypeEnum PartType { get; set; }		
	}

	public class ConMemberPosition
	{
		public ConMemberPlacementDefinitionTypeEnum DefinedBy { get; set; }

		/// <summary>
		/// Resolved global-frame origin of the member's local coordinate system.
		/// Populated by the server for every member regardless of <see cref="DefinedBy"/>.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Vector3D Origin { get; set; }

		/// <summary>
		/// Resolved global-frame X axis of the member's local coordinate system
		/// (the reference-line direction, from BEGIN toward END).
		/// Populated by the server for every member regardless of <see cref="DefinedBy"/>;
		/// previously was null for members placed via
		/// <see cref="ConMemberPlacementDefinitionTypeEnum.RotationsOfX"/>.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Vector3D AxisX { get; set; }

		/// <summary>
		/// Resolved global-frame Y axis of the member's local coordinate system
		/// (cross-section rotation about <see cref="AxisX"/>). Determines which physical face
		/// "Top flange 1" / "Bottom flange 1" map to on the host's box cross-section.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Vector3D AxisY { get; set; }

		/// <summary>
		/// Resolved global-frame Z axis of the member's local coordinate system.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Vector3D AxisZ { get; set; }

		// Original placement parameters (preserved for round-trip POST/PUT):

		// alpha
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public double? BetaDirection { get; set; }

		// beta
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public double? GamaPitch { get; set; }

		// rot
		public double AlphaRotation { get; set; }

		public double OffsetEx { get; set; }

		public double OffsetEy { get; set; }

		public double OffsetEz { get; set; }

		public ConMemberAlignmentTypeEnum Align { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConAlignedPlate AlignedPlate { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConAlignedPlate RelatedPlate { get; set; }
	}

	public class ConStiffnessAnalysis
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public double? TheoreticalLengthZ { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public double? TheoreticalLengthY { get; set; }
	}
}