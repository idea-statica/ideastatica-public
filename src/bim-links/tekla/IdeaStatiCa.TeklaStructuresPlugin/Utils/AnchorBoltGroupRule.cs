using CI.Geometry3D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utils
{
	/// <summary>Whether a bolt group is an anchor and how the base plate it holds goes out, judged on facts read from the model beforehand.</summary>
	internal static class AnchorBoltGroupRule
	{
		/// <summary>Millimetres. The length IDEA Connection gives a new anchor, used when the group states none.</summary>
		internal const double DefaultAnchorLength = 100.0;

		private static readonly double ParallelCos = Math.Cos(5.0 * Math.PI / 180.0);
		private static readonly double RiseCos = Math.Cos(45.0 * Math.PI / 180.0);

		/// <summary>The anchor's physical length: the bolt's catalog length, or the default when the group gives none.</summary>
		internal static double AnchorLength(double reportedLength) => reportedLength > 0 ? reportedLength : DefaultAnchorLength;

		/// <summary>
		/// The grid frame turned so its Z points away from the member standing on the plate - the side the anchors go to.
		/// A base plate component offers both bolting directions, so the bolt group's own Z can point into the column.
		/// Turning Z and Y together keeps the frame right-handed.
		/// </summary>
		internal static (Vector3D X, Vector3D Y, Vector3D Z) PointAwayFrom(Vector3D x, Vector3D y, Vector3D z, Vector3D columnRise)
			=> (z | columnRise) > 0 ? (x, -y, -z) : (x, y, z);

		/// <summary>
		/// A single-member joint without an anchor grid is discarded downstream, so a plate-profile member goes out as a
		/// plate only where an anchor holds it, and never as the joint's last structural member.
		/// </summary>
		internal static bool IsAnchoredPlate(AnchoredPlateFacts facts)
			=> facts.OtherStructuralMemberLeft
				&& (facts.BoltGroupAnchor || (facts.RodAnchorIsBeam && facts.RodAnchorInJoint && facts.RodDetailFirstGroupOnPlate));

		/// <summary>Which joints fold into another and which plates stay members, so that no plate leaves as a plate in one joint and as a member in another.</summary>
		internal static ColumnBaseFoldPlan PlanColumnBaseFolds(IReadOnlyList<JointPlateReadings> joints)
		{
			var plan = new ColumnBaseFoldPlan();
			var homeOf = new Dictionary<Guid, int>();
			for (var index = 0; index < joints.Count; index++)
			{
				foreach (var plate in joints[index].ExportedPlates)
				{
					if (!homeOf.ContainsKey(plate))
					{
						homeOf[plate] = index;
					}
				}
			}

			for (var second = 0; second < joints.Count; second++)
			{
				var kept = joints[second].MemberPlates;
				if (kept.Count == 0 || kept.Count != joints[second].StructuralMemberCount)
				{
					continue;
				}

				var homes = kept.Select(plate => homeOf.TryGetValue(plate, out var home) ? home : -1).Distinct().ToList();
				if (homes.Count == 1 && homes[0] >= 0 && homes[0] != second)
				{
					plan.FoldInto[second] = homes[0];
				}
			}

			var demoted = new HashSet<Guid>();
			for (var second = 0; second < joints.Count; second++)
			{
				if (plan.FoldInto.ContainsKey(second))
				{
					continue;
				}

				foreach (var plate in joints[second].MemberPlates)
				{
					if (homeOf.TryGetValue(plate, out var home) && home != second && demoted.Add(plate))
					{
						plan.StayMembers.Add((plate, home));
					}
				}
			}

			return plan;
		}

		internal static AnchorBoltGroupVerdict Judge(AnchorBoltGroupFacts facts)
		{
			if (!facts.FastensOnePartOnly)
			{
				return AnchorBoltGroupVerdict.No("fastens more than one part");
			}

			if (!facts.PartIsPlate)
			{
				return AnchorBoltGroupVerdict.No("the part it fastens is not a plate");
			}

			if (facts.InRodAnchorDetail)
			{
				return AnchorBoltGroupVerdict.No("its detail models the anchors as rods");
			}

			if (!facts.IsBolt)
			{
				var holes = JudgeHoles(facts);
				if (holes != null)
				{
					return holes;
				}
			}

			if (facts.BoltAxis.Magnitude <= 0)
			{
				return AnchorBoltGroupVerdict.No("no bolt axis");
			}

			var axis = facts.BoltAxis.Normalize;
			var reached = MemberCheck.NoneWelded;
			foreach (var member in facts.WeldedMembers ?? new List<AnchorBoltGroupFacts.WeldedMember>())
			{
				var lower = member.Begin.Z <= member.End.Z ? member.Begin : member.End;
				var upper = ReferenceEquals(lower, member.Begin) ? member.End : member.Begin;
				var along = new Vector3D(upper.X - lower.X, upper.Y - lower.Y, upper.Z - lower.Z);
				if (along.Magnitude <= 0)
				{
					continue;
				}

				var rise = along.Normalize;
				if (Math.Abs(rise | axis) < ParallelCos)
				{
					reached = Max(reached, MemberCheck.NotAlongBolts);
					continue;
				}

				if (rise.DirectionZ < RiseCos)
				{
					reached = Max(reached, MemberCheck.DoesNotRise);
					continue;
				}

				if (!IsOnPlate(lower, facts))
				{
					reached = Max(reached, MemberCheck.LowerEndOffPlate);
					continue;
				}

				if (!facts.IsBolt && member.Id != facts.DetailPrimaryId)
				{
					reached = Max(reached, MemberCheck.NotTheDetailsColumn);
					continue;
				}

				return AnchorBoltGroupVerdict.Yes(rise, member.Label, fromHoles: !facts.IsBolt);
			}

			return AnchorBoltGroupVerdict.No(Describe(reached));
		}

		/// <summary>The narrow rule holes pass before the geometry; null when they may go on to it.</summary>
		private static AnchorBoltGroupVerdict JudgeHoles(AnchorBoltGroupFacts facts)
		{
			if (facts.PositionCount < 2)
			{
				return AnchorBoltGroupVerdict.No("a single hole");
			}

			if (facts.OtherAnchorGroupsOnPlate > 0)
			{
				return AnchorBoltGroupVerdict.No("holes beside another group that could be the plate's anchors");
			}

			if (string.IsNullOrEmpty(facts.DetailPrimaryId))
			{
				return AnchorBoltGroupVerdict.No("holes not made by a detail");
			}

			return null;
		}

		private static bool IsOnPlate(IPoint3D point, AnchorBoltGroupFacts facts)
		{
			var tolerance = facts.PlateThickness;

			return point.X >= facts.PlateMin.X - tolerance && point.X <= facts.PlateMax.X + tolerance
				&& point.Y >= facts.PlateMin.Y - tolerance && point.Y <= facts.PlateMax.Y + tolerance
				&& point.Z >= facts.PlateMin.Z - tolerance && point.Z <= facts.PlateMax.Z + tolerance;
		}

		private static MemberCheck Max(MemberCheck a, MemberCheck b) => a > b ? a : b;

		private static string Describe(MemberCheck reached)
		{
			switch (reached)
			{
				case MemberCheck.NotAlongBolts:
					return "no welded member runs along the bolts";
				case MemberCheck.DoesNotRise:
					return "no member along the bolts rises from the plate";
				case MemberCheck.LowerEndOffPlate:
					return "no rising member ends on the plate";
				case MemberCheck.NotTheDetailsColumn:
					return "the member standing on the plate is not the one the holes' detail is attached to";
				default:
					return "no frame member is welded to the plate";
			}
		}

		/// <summary>How far the best welded member got, so a rejection names the criterion that stopped it.</summary>
		private enum MemberCheck
		{
			NoneWelded,
			NotAlongBolts,
			DoesNotRise,
			LowerEndOffPlate,
			NotTheDetailsColumn,
		}
	}

	/// <summary>
	/// What <see cref="AnchorBoltGroupRule"/> judges a bolt group on. Millimetres and model coordinates, as the link reads
	/// them.
	/// </summary>
	internal sealed class AnchorBoltGroupFacts
	{
		public bool IsBolt { get; set; }

		/// <summary>The group names one part in both bolting slots and no other part.</summary>
		public bool FastensOnePartOnly { get; set; }

		public bool PartIsPlate { get; set; }

		/// <summary>The group or its plate belongs to a detail whose rods and bolt group make an anchor grid of their own.</summary>
		public bool InRodAnchorDetail { get; set; }

		public int PositionCount { get; set; }

		/// <summary>
		/// Other groups on the same plate that fasten it alone and could be its anchors: bolts, or two holes and more.
		/// Read for a group of holes only.
		/// </summary>
		public int OtherAnchorGroupsOnPlate { get; set; }

		/// <summary>The part the detail that made the group is attached to; read for a group of holes only.</summary>
		public string DetailPrimaryId { get; set; }

		public Vector3D BoltAxis { get; set; }

		public IPoint3D PlateMin { get; set; }

		public IPoint3D PlateMax { get; set; }

		public double PlateThickness { get; set; }

		/// <summary>Frame members welded to the plate, by the physical ends of their centre lines.</summary>
		public List<WeldedMember> WeldedMembers { get; set; }

		internal sealed class WeldedMember
		{
			public WeldedMember(string id, IPoint3D begin, IPoint3D end, string label)
			{
				Id = id;
				Begin = begin;
				End = end;
				Label = label;
			}

			public string Id { get; }

			public IPoint3D Begin { get; }

			public IPoint3D End { get; }

			public string Label { get; }
		}
	}

	/// <summary>How one joint read its plate-profile members, as <see cref="AnchorBoltGroupRule.PlanColumnBaseFolds"/> needs it.</summary>
	internal sealed class JointPlateReadings
	{
		public JointPlateReadings(int structuralMemberCount, IReadOnlyCollection<Guid> exportedPlates, IReadOnlyCollection<Guid> memberPlates)
		{
			StructuralMemberCount = structuralMemberCount;
			ExportedPlates = exportedPlates;
			MemberPlates = memberPlates;
		}

		/// <summary>Members the joint exports as structural, plate-profile ones included.</summary>
		public int StructuralMemberCount { get; }

		public IReadOnlyCollection<Guid> ExportedPlates { get; }

		/// <summary>Plate-profile members the joint exports as members.</summary>
		public IReadOnlyCollection<Guid> MemberPlates { get; }
	}

	internal sealed class ColumnBaseFoldPlan
	{
		/// <summary>Joint index -> the index of the joint it folds into.</summary>
		public Dictionary<int, int> FoldInto { get; } = new Dictionary<int, int>();

		/// <summary>Plates that stay members in the joint that would have exported them as plates.</summary>
		public List<(Guid Plate, int Home)> StayMembers { get; } = new List<(Guid Plate, int Home)>();
	}

	/// <summary>What <see cref="AnchorBoltGroupRule.IsAnchoredPlate"/> decides a joint's plate-profile member on.</summary>
	internal sealed class AnchoredPlateFacts
	{
		/// <summary>A bolt group on the plate is an anchor modelled as bolts.</summary>
		public bool BoltGroupAnchor { get; set; }

		/// <summary>The plate's detail models its anchors as rods and the rod the export picks is a beam.</summary>
		public bool RodAnchorIsBeam { get; set; }

		/// <summary>That rod is among this joint's members, so its anchor grid goes out with this joint.</summary>
		public bool RodAnchorInJoint { get; set; }

		/// <summary>The detail's first bolt group fastens this plate - the group the rod importer takes its operand from.</summary>
		public bool RodDetailFirstGroupOnPlate { get; set; }

		public bool OtherStructuralMemberLeft { get; set; }
	}

	internal sealed class AnchorBoltGroupVerdict
	{
		private AnchorBoltGroupVerdict(bool isAnchor, string reason, Vector3D? columnRise, string column)
		{
			IsAnchor = isAnchor;
			Reason = reason;
			ColumnRise = columnRise;
			Column = column;
		}

		public bool IsAnchor { get; }

		public string Reason { get; }

		/// <summary>Unit direction from the plate up the member standing on it; null when the group is not an anchor.</summary>
		public Vector3D? ColumnRise { get; }

		public string Column { get; }

		internal static AnchorBoltGroupVerdict Yes(Vector3D columnRise, string column, bool fromHoles)
			=> new AnchorBoltGroupVerdict(
				true,
				fromHoles ? "holes under the foot of the column their detail is attached to" : "a frame member stands on the plate along the bolts",
				columnRise,
				column);

		internal static AnchorBoltGroupVerdict No(string reason) => new AnchorBoltGroupVerdict(false, reason, null, null);
	}
}
