using CI.Geometry3D;
using FluentAssertions;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.TeklaStructuresTest
{
	/// <summary>
	/// When <see cref="AnchorBoltGroupRule"/> takes a bolt group for anchors: bolts fastening a single plate that a frame
	/// member stands on, along the bolts. Millimetres, as the link reads Tekla model coordinates. The plate is PL19.1*360
	/// lying flat with its top face at z = 0, as under a column foot.
	/// </summary>
	public class AnchorBoltGroupRuleTest
	{
		private static readonly Vector3D Down = new Vector3D(0, 0, -1);

		[Test]
		public void ColumnStandingOnThePlate_IsAnAnchor()
		{
			var verdict = AnchorBoltGroupRule.Judge(ColumnBase(Down, Member(from: new Point3D(0, 0, 0), to: new Point3D(0, 0, 4000))));

			verdict.IsAnchor.Should().BeTrue(verdict.Reason);
			verdict.ColumnRise.Value.DirectionZ.Should().BeApproximately(1, 1e-9);
		}

		/// <summary>
		/// A base plate component lets the user pick either bolting direction, so the bolt axis can point into the column
		/// as well as away from it.
		/// </summary>
		[Test]
		public void BoltAxisPointingUp_IsStillAnAnchor()
		{
			var verdict = AnchorBoltGroupRule.Judge(ColumnBase(new Vector3D(0, 0, 1), Member(from: new Point3D(0, 0, 0), to: new Point3D(0, 0, 4000))));

			verdict.IsAnchor.Should().BeTrue(verdict.Reason);
		}

		[Test]
		public void InclinedColumn_WithTheBoltsAlongIt_IsAnAnchor()
		{
			var tilt = 20.0 * Math.PI / 180.0;
			var top = new Point3D(4000 * Math.Sin(tilt), 0, 4000 * Math.Cos(tilt));

			var verdict = AnchorBoltGroupRule.Judge(ColumnBase(new Vector3D(-Math.Sin(tilt), 0, -Math.Cos(tilt)), Member(from: new Point3D(0, 0, 0), to: top)));

			verdict.IsAnchor.Should().BeTrue(verdict.Reason);
		}

		/// <summary>
		/// A column end a few millimetres into the plate - a fitted end, or one stopping on grout below the plate top - still
		/// stands on it: the end may lie up to one plate thickness outside the plate's box.
		/// </summary>
		[Test]
		public void LowerEndWithinOnePlateThicknessOfThePlate_StillStandsOnIt()
		{
			var verdict = AnchorBoltGroupRule.Judge(ColumnBase(Down, Member(from: new Point3D(0, 0, -25), to: new Point3D(0, 0, 4000))));

			verdict.IsAnchor.Should().BeTrue(verdict.Reason);
		}

		/// <summary>A seat or corbel plate part-way up a column: the column is along the bolts, but it does not end there.</summary>
		[Test]
		public void SeatPlatePartWayUpAColumn_IsNotAnAnchor()
		{
			var verdict = AnchorBoltGroupRule.Judge(ColumnBase(Down, Member(from: new Point3D(0, 0, -3000), to: new Point3D(0, 0, 3000))));

			verdict.IsAnchor.Should().BeFalse();
			verdict.Reason.Should().Be("no rising member ends on the plate");
		}

		/// <summary>A cap plate: the column ends at the plate, but from below, so its lower end is metres away.</summary>
		[Test]
		public void CapPlateOnTopOfAColumn_IsNotAnAnchor()
		{
			var verdict = AnchorBoltGroupRule.Judge(ColumnBase(Down, Member(from: new Point3D(0, 0, -4000), to: new Point3D(0, 0, -19.1))));

			verdict.IsAnchor.Should().BeFalse();
			verdict.Reason.Should().Be("no rising member ends on the plate");
		}

		/// <summary>An end plate on a beam anchored into a wall: along the bolts, but it does not rise.</summary>
		[Test]
		public void HorizontalBeamEndPlate_IsNotAnAnchor()
		{
			var verdict = AnchorBoltGroupRule.Judge(ColumnBase(new Vector3D(-1, 0, 0), Member(from: new Point3D(0, 0, 0), to: new Point3D(4000, 0, 0))));

			verdict.IsAnchor.Should().BeFalse();
			verdict.Reason.Should().Be("no member along the bolts rises from the plate");
		}

		[Test]
		public void MemberNotAlongTheBolts_IsNotAnAnchor()
		{
			var verdict = AnchorBoltGroupRule.Judge(ColumnBase(new Vector3D(1, 0, 0), Member(from: new Point3D(0, 0, 0), to: new Point3D(0, 0, 4000))));

			verdict.IsAnchor.Should().BeFalse();
			verdict.Reason.Should().Be("no welded member runs along the bolts");
		}

		[Test]
		public void NoMemberWeldedToThePlate_IsNotAnAnchor()
		{
			var verdict = AnchorBoltGroupRule.Judge(ColumnBase(Down));

			verdict.IsAnchor.Should().BeFalse();
			verdict.Reason.Should().Be("no frame member is welded to the plate");
		}

		/// <summary>
		/// A detail with rod anchors carries the holes for the rods on its base plate - holes that would otherwise pass.
		/// The rod importer builds that anchor grid, so the holes must not give a second one.
		/// </summary>
		[Test]
		public void HolesInARodAnchorDetail_AreNotAnAnchorOfTheirOwn()
		{
			var facts = HolesUnderTheDetailsColumn();
			facts.InRodAnchorDetail = true;

			AnchorBoltGroupRule.Judge(facts).IsAnchor.Should().BeFalse();
		}

		/// <summary>A base plate detail that leaves only holes for its anchors: the holes are where the anchors go.</summary>
		[Test]
		public void HolesUnderTheFootOfTheColumnTheirDetailIsAttachedTo_AreAnAnchor()
		{
			var verdict = AnchorBoltGroupRule.Judge(HolesUnderTheDetailsColumn());

			verdict.IsAnchor.Should().BeTrue(verdict.Reason);
		}

		/// <summary>A single hole is a grout or vent hole, not an anchor pattern.</summary>
		[Test]
		public void SingleHole_IsNotAnAnchor()
		{
			var facts = HolesUnderTheDetailsColumn();
			facts.PositionCount = 1;

			AnchorBoltGroupRule.Judge(facts).IsAnchor.Should().BeFalse();
		}

		/// <summary>Real bolts or other holes on the same plate could be its anchors; the holes must not add a second grid.</summary>
		[Test]
		public void HolesBesideAnotherAnchorGroupOnThePlate_AreNotAnAnchor()
		{
			var facts = HolesUnderTheDetailsColumn();
			facts.OtherAnchorGroupsOnPlate = 1;

			AnchorBoltGroupRule.Judge(facts).IsAnchor.Should().BeFalse();
		}

		[Test]
		public void HolesNotMadeByADetail_AreNotAnAnchor()
		{
			var facts = HolesUnderTheDetailsColumn();
			facts.DetailPrimaryId = null;

			AnchorBoltGroupRule.Judge(facts).IsAnchor.Should().BeFalse();
		}

		[Test]
		public void HolesWhoseDetailIsAttachedToAnotherPart_AreNotAnAnchor()
		{
			var facts = HolesUnderTheDetailsColumn();
			facts.DetailPrimaryId = "another part";

			var verdict = AnchorBoltGroupRule.Judge(facts);

			verdict.IsAnchor.Should().BeFalse();
			verdict.Reason.Should().Be("the member standing on the plate is not the one the holes' detail is attached to");
		}

		/// <summary>Only holes are held to the narrow rule; real bolts need no detail and no hole count.</summary>
		[Test]
		public void BoltsOutsideADetail_AreStillAnAnchor()
		{
			var facts = ColumnBase(Down, Member(from: new Point3D(0, 0, 0), to: new Point3D(0, 0, 4000)));
			facts.DetailPrimaryId = null;

			var verdict = AnchorBoltGroupRule.Judge(facts);

			verdict.IsAnchor.Should().BeTrue(verdict.Reason);
		}

		/// <summary>
		/// Two plates, or a plate and a plate washer: a group of any shape but the one the part walk skips could be reached
		/// by that walk as well, and exported twice.
		/// </summary>
		[Test]
		public void GroupFasteningMoreThanOnePart_IsNotAnAnchor()
		{
			var facts = ColumnBase(Down, Member(from: new Point3D(0, 0, 0), to: new Point3D(0, 0, 4000)));
			facts.FastensOnePartOnly = false;

			AnchorBoltGroupRule.Judge(facts).IsAnchor.Should().BeFalse();
		}

		[Test]
		public void FastenedPartNotAPlate_IsNotAnAnchor()
		{
			var facts = ColumnBase(Down, Member(from: new Point3D(0, 0, 0), to: new Point3D(0, 0, 4000)));
			facts.PartIsPlate = false;

			AnchorBoltGroupRule.Judge(facts).IsAnchor.Should().BeFalse();
		}

		[Test]
		public void AnchorLength_IsTheBoltsCatalogLength()
		{
			AnchorBoltGroupRule.AnchorLength(reportedLength: 450).Should().Be(450);
		}

		/// <summary>A zero length would give a zero-length anchor and a foundation block with no depth.</summary>
		[TestCase(0.0)]
		[TestCase(-1.0)]
		public void AnchorLength_WithoutOne_IsTheDefaultAnchorLength(double reportedLength)
		{
			AnchorBoltGroupRule.AnchorLength(reportedLength).Should().Be(AnchorBoltGroupRule.DefaultAnchorLength);
		}

		/// <summary>Bolting direction 2 of a base plate component: the group's Z points up into the column.</summary>
		[Test]
		public void FramePointingIntoTheColumn_IsTurnedToPointAwayFromIt()
		{
			var frame = AnchorBoltGroupRule.PointAwayFrom(
				new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), columnRise: new Vector3D(0, 0, 1));

			frame.Z.DirectionZ.Should().BeApproximately(-1, 1e-9);
			ShouldBeRightHanded(frame);
		}

		[Test]
		public void FramePointingAwayFromTheColumn_IsKept()
		{
			var frame = AnchorBoltGroupRule.PointAwayFrom(
				new Vector3D(1, 0, 0), new Vector3D(0, -1, 0), Down, columnRise: new Vector3D(0, 0, 1));

			frame.Y.DirectionY.Should().BeApproximately(-1, 1e-9);
			frame.Z.DirectionZ.Should().BeApproximately(-1, 1e-9);
			ShouldBeRightHanded(frame);
		}

		[Test]
		public void PlateBoltedByAnAnchorGroup_IsExportedAsAPlate()
		{
			AnchorBoltGroupRule.IsAnchoredPlate(AnchoredPlate(boltGroupAnchor: true)).Should().BeTrue();
		}

		[Test]
		public void PlateOfARodDetailWhoseRodIsInTheJoint_IsExportedAsAPlate()
		{
			AnchorBoltGroupRule.IsAnchoredPlate(AnchoredPlate(rodIsBeam: true, rodInJoint: true, firstGroupOnPlate: true)).Should().BeTrue();
		}

		/// <summary>
		/// A partial selection can leave the rod out of the joint; its grid would then go out elsewhere or not at all, and
		/// the column base would be discarded as a single-member joint.
		/// </summary>
		[Test]
		public void PlateOfARodDetailWhoseRodIsNotInTheJoint_StaysAMember()
		{
			AnchorBoltGroupRule.IsAnchoredPlate(AnchoredPlate(rodIsBeam: true, rodInJoint: false, firstGroupOnPlate: true)).Should().BeFalse();
		}

		/// <summary>Only a beam rod is exported as an anchor grid; a polybeam rod goes out as a member.</summary>
		[Test]
		public void PlateOfARodDetailWhoseRodIsNotABeam_StaysAMember()
		{
			AnchorBoltGroupRule.IsAnchoredPlate(AnchoredPlate(rodIsBeam: false, rodInJoint: true, firstGroupOnPlate: true)).Should().BeFalse();
		}

		/// <summary>The rod grid takes its operands from the detail's first bolt group; on another part it names another plate.</summary>
		[Test]
		public void PlateOfARodDetailWhoseFirstGroupIsElsewhere_StaysAMember()
		{
			AnchorBoltGroupRule.IsAnchoredPlate(AnchoredPlate(rodIsBeam: true, rodInJoint: true, firstGroupOnPlate: false)).Should().BeFalse();
		}

		[Test]
		public void AnchoredPlateThatIsTheLastStructuralMember_StaysAMember()
		{
			var facts = AnchoredPlate(boltGroupAnchor: true);
			facts.OtherStructuralMemberLeft = false;

			AnchorBoltGroupRule.IsAnchoredPlate(facts).Should().BeFalse();
		}

		[Test]
		public void PlateWithoutAnAnchor_StaysAMember()
		{
			AnchorBoltGroupRule.IsAnchoredPlate(AnchoredPlate()).Should().BeFalse();
		}

		/// <summary>
		/// The measured case on MC-TIC-612-004: the sorter seeds a second joint at the base plate's own node, where the
		/// plate is the only structural member. Exported apart it would send the plate once as a plate and once as a member.
		/// </summary>
		[Test]
		public void SecondSightingOfAColumnBase_FoldsIntoTheJointThatExportsThePlate()
		{
			var plan = AnchorBoltGroupRule.PlanColumnBaseFolds(new[]
			{
				Readings(structural: 1, exported: new[] { BasePlate }),
				Readings(structural: 1, keptMembers: new[] { BasePlate }),
			});

			plan.FoldInto.Should().Equal(new Dictionary<int, int> { [1] = 0 });
			plan.StayMembers.Should().BeEmpty();
		}

		[Test]
		public void PlateKeptAMemberInItsOnlyJoint_IsLeftAlone()
		{
			var plan = AnchorBoltGroupRule.PlanColumnBaseFolds(new[] { Readings(structural: 1, keptMembers: new[] { BasePlate }) });

			plan.FoldInto.Should().BeEmpty();
			plan.StayMembers.Should().BeEmpty();
		}

		/// <summary>A second sighting that holds something else structural is not folded; the plate then stays a member everywhere.</summary>
		[Test]
		public void SecondSightingWithAnotherStructuralMember_KeepsThePlateAMemberInTheFirstJointToo()
		{
			var plan = AnchorBoltGroupRule.PlanColumnBaseFolds(new[]
			{
				Readings(structural: 1, exported: new[] { BasePlate }),
				Readings(structural: 2, keptMembers: new[] { BasePlate }),
			});

			plan.FoldInto.Should().BeEmpty();
			plan.StayMembers.Should().Equal((BasePlate, 0));
		}

		[Test]
		public void SecondSightingOfPlatesExportedByDifferentJoints_IsNotFolded()
		{
			var other = Guid.NewGuid();

			var plan = AnchorBoltGroupRule.PlanColumnBaseFolds(new[]
			{
				Readings(structural: 1, exported: new[] { BasePlate }),
				Readings(structural: 1, exported: new[] { other }),
				Readings(structural: 2, keptMembers: new[] { BasePlate, other }),
			});

			plan.FoldInto.Should().BeEmpty();
			plan.StayMembers.Should().BeEquivalentTo(new[] { (BasePlate, 0), (other, 1) });
		}

		private static readonly Guid BasePlate = Guid.NewGuid();

		private static JointPlateReadings Readings(int structural, Guid[] exported = null, Guid[] keptMembers = null)
			=> new JointPlateReadings(structural, exported ?? new Guid[0], keptMembers ?? new Guid[0]);

		private static AnchoredPlateFacts AnchoredPlate(bool boltGroupAnchor = false, bool rodIsBeam = false, bool rodInJoint = false, bool firstGroupOnPlate = false)
			=> new AnchoredPlateFacts
			{
				BoltGroupAnchor = boltGroupAnchor,
				RodAnchorIsBeam = rodIsBeam,
				RodAnchorInJoint = rodInJoint,
				RodDetailFirstGroupOnPlate = firstGroupOnPlate,
				OtherStructuralMemberLeft = true,
			};

		private static void ShouldBeRightHanded((Vector3D X, Vector3D Y, Vector3D Z) frame)
		{
			var cross = frame.X * frame.Y;

			(cross | frame.Z).Should().BeApproximately(1, 1e-9);
		}

		private static AnchorBoltGroupFacts ColumnBase(Vector3D boltAxis, params AnchorBoltGroupFacts.WeldedMember[] welded)
			=> new AnchorBoltGroupFacts
			{
				IsBolt = true,
				FastensOnePartOnly = true,
				PartIsPlate = true,
				InRodAnchorDetail = false,
				BoltAxis = boltAxis,
				PlateMin = new Point3D(-180, -180, -19.1),
				PlateMax = new Point3D(180, 180, 0),
				PlateThickness = 19.1,
				WeldedMembers = new List<AnchorBoltGroupFacts.WeldedMember>(welded),
			};

		/// <summary>The measured column base of component 1014 on MC-TIC-612-004: four holes, no bolt, made by the detail on the column.</summary>
		private static AnchorBoltGroupFacts HolesUnderTheDetailsColumn()
		{
			var facts = ColumnBase(Down, Member(from: new Point3D(0, 0, 0), to: new Point3D(0, 0, 4000)));
			facts.IsBolt = false;
			facts.PositionCount = 4;
			facts.OtherAnchorGroupsOnPlate = 0;
			facts.DetailPrimaryId = ColumnId;

			return facts;
		}

		private const string ColumnId = "column";

		private static AnchorBoltGroupFacts.WeldedMember Member(IPoint3D from, IPoint3D to) => new AnchorBoltGroupFacts.WeldedMember(ColumnId, from, to, "member");
	}
}
