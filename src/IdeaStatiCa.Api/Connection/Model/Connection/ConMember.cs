﻿using IdeaRS.OpenModel.Geometry3D;
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

		public bool? MirrorY { get; set; }

		public bool? MirrorZ { get; set; }

		public bool IsBearing { get; }

		public ConMemberPosition Position { get; set; }

		public ConMemberModel Model { get; set; }

	}

	public class ConMemberModel
	{
		public string ModelType { get; set; }

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

		public string PartType { get; set; }		
	}

	public class ConMemberPosition
	{
		public ConMemberPlacementDefinitionTypeEnum DefinedBy { get; set; }

		// for 3D vector
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Vector3D AxisX { get; set; } = null;

		// for Rotations

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

	public enum ConAlignedPlateSideCodeEnum : int
	{
		Default = 0,
		LowerSide = 1,
		UpperSide = 2,
		BothSides = 3,
		Center = 4
	}

	public enum ConMemberForcesInEnum
	{
		Position = 0,
		Node = 1,
		Bolts = 2,
		SelectedMemberFace = 3,
	}

	public enum ConMemberPlacementDefinitionTypeEnum
	{
		None = 0,
		DirectionVector = 1,
		RotationsOfX = 2,
		Lcs,
	}

	public enum ConMemberAlignmentTypeEnum
	{
		None,
		ToMemberPlate,
		ToMemberPlateRotateThenTranslate,
	}

}