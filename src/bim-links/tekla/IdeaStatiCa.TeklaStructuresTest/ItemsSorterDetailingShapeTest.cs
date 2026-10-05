using CI.Geometry3D;
using FluentAssertions;
using IdeaStatiCa.BIM.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace IdeaStatiCa.TeklaStructuresTest
{
	/// <summary>
	/// How <see cref="ItemsSorter"/> tells connection detailing from a frame member, and how it places fabrication
	/// the node box does not reach. Millimetres, as everywhere the
	/// sorter reads Tekla model coordinates.
	/// </summary>
	public class ItemsSorterDetailingShapeTest
	{
		/// <summary>
		/// <see cref="Item.CustomComparer"/> is process-wide and <c>BulkSelectionHelper.FindJoints</c> replaces it with
		/// one that reads a Tekla <c>ModelObject.Identifier</c> off every parent. These parts are plain sentinels, so a
		/// test running after one that went through FindJoints would see every comparison answer false. Each test
		/// therefore states the identity it sorts under.
		/// </summary>
		[SetUp]
		public void CompareItemsByTheirParent() => Item.CustomComparer = new ParentIdentityComparer();

		[Test]
		public void ShortWidePlateBeam_BecomesStiffeningMember()
		{
			var gusset = MakeMember("gusset", from: Origin, to: new Point3D(0, 363, 0), cssWidth: 25, cssHeight: 1485);

			var joint = SortFrameWith(gusset).Joints.Single();

			joint.StiffeningMembers.Should().Contain(gusset);
			joint.Members.Should().NotContain(gusset);
		}

		[Test]
		public void ShortWidePlateBeam_DoesNotFormAJointOfItsOwn()
		{
			var gusset = MakeMember("gusset", from: Origin, to: new Point3D(0, 363, 0), cssWidth: 25, cssHeight: 1485);

			var result = SortFrameWith(gusset);

			result.Joints.Should().HaveCount(1);
		}

		/// <summary>
		/// A gusset is routinely wider than the box that found the bolts through it, so the box alone never reaches it
		/// and - being untaken - never widens the box toward itself either. The bolts also pass through the member the
		/// gusset is bolted to, and holding THAT is what says the gusset is here. The fastener sits as far out as the
		/// gusset, so where it sits cannot be the anchor.
		/// </summary>
		[Test]
		public void PlateOutsideTheBox_IsTakenFromTheFastenerThatClampsItToAHeldMember()
		{
			var frame = Frame();
			var gusset = MakePlate("gusset", at: new Point3D(0, 1500, 0), halfSize: 700);
			var bolts = MakeFastener("bolts", new Point3D(0, 1500, 0), frame[0], gusset);

			var joint = SortFrame(frame, new[] { gusset }, new[] { bolts }).Joints.Single();

			joint.Plates.Should().Contain(gusset);
		}

		/// <summary>
		/// The counterpart: nothing the joint holds is clamped, so the plate is not this joint's to take. Holding a
		/// clamped part is the whole bound on reach - without it the rule reads as "any bolted plate anywhere".
		/// </summary>
		[Test]
		public void PlateBoltedToNothingTheJointHolds_IsNotTaken()
		{
			var frame = Frame();
			var stranger = MakePlate("stranger", at: new Point3D(0, 9000, 0), halfSize: 700);
			var bolts = MakeFastener("bolts", new Point3D(0, 9000, 0), stranger);

			var joint = SortFrame(frame, new[] { stranger }, new[] { bolts }).Joints.Single();

			joint.Plates.Should().NotContain(stranger);
		}

		/// <summary>
		/// A bolt group names whatever the source models the part as, and a plate-profile beam arrives as a member.
		/// Only the leftover plate pool is searched, and <see cref="Item.CustomComparer"/> reads Parent off both sides,
		/// so a member reaching that search as a null plate took the whole import down.
		/// </summary>
		[Test]
		public void MemberClampedByTheFastener_IsTakenAsDetailing()
		{
			var frame = Frame();
			var coverPlate = MakeMember("cover-plate", from: new Point3D(0, 1500, 0), to: new Point3D(0, 1760, 0), cssWidth: 20, cssHeight: 520);
			var gusset = MakePlate("gusset", at: new Point3D(0, 1500, 0), halfSize: 700);
			var bolts = MakeFastener("bolts", new Point3D(0, 1500, 0), frame[0], coverPlate, gusset);

			var joint = SortFrame(frame.Append(coverPlate).ToArray(), new[] { gusset }, new[] { bolts }).Joints.Single();

			joint.Plates.Should().Contain(gusset);
			joint.StiffeningMembers.Should().Contain(coverPlate);
		}

		/// <summary>
		/// A bolt group is one physical thing in one place. Plates cannot be claimed twice - they leave the pool as
		/// they are taken - but fasteners have no pool, so a group clamping a member that runs THROUGH several nodes
		/// is offered to every one of them.
		/// </summary>
		[Test]
		public void FastenerClampingAThroughMember_IsClaimedByOneJointOnly()
		{
			// One column through two storeys, a beam framing in at each - two joints sharing the column.
			var column = MakeMember("column", from: new Point3D(0, 0, -4000), to: new Point3D(0, 0, 4000), cssWidth: 300, cssHeight: 400);
			var lower = MakeMember("lower", from: new Point3D(0, 0, -4000), to: new Point3D(8000, 0, -4000), cssWidth: 200, cssHeight: 500);
			var upper = MakeMember("upper", from: new Point3D(0, 0, 4000), to: new Point3D(8000, 0, 4000), cssWidth: 200, cssHeight: 500);
			var bolts = MakeFastener("bolts", new Point3D(0, 0, -4000), column);

			var data = new SorterData
			{
				Members = new List<Member> { column, lower, upper },
				Plates = new List<Plate>(),
				Welds = new List<Weld>(),
				Fasteners = new List<FastenerGrid> { bolts },
			};

			var result = new ItemsSorter().Sort(data, ModelCoordinatorSettings);

			result.Joints.Should().HaveCountGreaterThan(1);
			result.Joints.Count(j => j.Fasteners.Contains(bolts)).Should().Be(1);
		}

		/// <summary>
		/// Reference placement must not outrank geometry. Joints are built in descending member order, so a joint that
		/// merely holds the through column is reached before the one the bolts actually sit in - and taking them there
		/// would take the plates they clamp with them, leaving the right joint without either.
		/// </summary>
		[Test]
		public void FastenerInsideANodeBox_GoesToTheJointItSitsIn_NotToOneThatMerelyHoldsTheMember()
		{
			var column = MakeMember("column", from: new Point3D(0, 0, -4000), to: new Point3D(0, 0, 4000), cssWidth: 300, cssHeight: 400);
			var lower = MakeMember("lower", from: new Point3D(0, 0, -4000), to: new Point3D(8000, 0, -4000), cssWidth: 200, cssHeight: 500);
			// Two beams at the upper node, one at the lower, so the upper joint is built first.
			var upperEast = MakeMember("upper-east", from: new Point3D(0, 0, 4000), to: new Point3D(8000, 0, 4000), cssWidth: 200, cssHeight: 500);
			var upperWest = MakeMember("upper-west", from: new Point3D(0, 0, 4000), to: new Point3D(-8000, 0, 4000), cssWidth: 200, cssHeight: 500);
			var bolts = MakeFastener("bolts", new Point3D(0, 0, -4000), column);

			var data = new SorterData
			{
				Members = new List<Member> { column, lower, upperEast, upperWest },
				Plates = new List<Plate>(),
				Welds = new List<Weld>(),
				Fasteners = new List<FastenerGrid> { bolts },
			};

			var result = new ItemsSorter().Sort(data, ModelCoordinatorSettings);

			var holder = result.Joints.Single(j => j.Fasteners.Contains(bolts));
			holder.Members.Should().Contain(lower);
		}

		/// <summary>
		/// A node is not a joint until it survives the assembly loop: it can be absorbed into an earlier joint when a
		/// detailing part ends there. A fastener sitting in such a node's box is left to it by the geometry rule and
		/// would then reach nobody, so it gets one more pass against what the joints actually hold.
		/// </summary>
		[Test]
		public void FastenerLeftToANodeThatNeverBecameAJoint_IsStillPlaced()
		{
			var column = MakeMember("column", from: new Point3D(0, 0, -7500), to: Origin, cssWidth: 300, cssHeight: 400);
			var beam = MakeMember("beam", from: Origin, to: new Point3D(8000, 0, 0), cssWidth: 200, cssHeight: 500);
			// Short and wide, so it is detailing - and its far end takes the node there out of the running.
			var gusset = MakeMember("gusset", from: Origin, to: new Point3D(0, 300, 0), cssWidth: 25, cssHeight: 1485);
			// The bolts sit at that far end, clamping the column the first joint holds.
			var bolts = MakeFastener("bolts", new Point3D(0, 300, 0), column);

			var data = new SorterData
			{
				Members = new List<Member> { column, beam, gusset },
				Plates = new List<Plate>(),
				Welds = new List<Weld>(),
				Fasteners = new List<FastenerGrid> { bolts },
			};

			var result = new ItemsSorter().Sort(data, ModelCoordinatorSettings);

			result.Joints.Count(j => j.Fasteners.Contains(bolts)).Should().Be(1);
		}

		/// <summary>
		/// The last pass hands a joint the plates a fastener clamps after the joint's welds were collected, so a weld on
		/// such a plate has to be collected again, or it stays out of the joint that holds both of its items.
		/// </summary>
		[Test]
		public void PlateTheLastPassTakes_BringsItsWeldIntoTheJoint()
		{
			var frame = Frame();
			// Outside the node box, but within the reach of the weld that ties it to the beam.
			var gusset = MakePlate("gusset", at: new Point3D(0, 300, 0), halfSize: 100);
			// Welded only to each other, so no weld leads to either from anything a joint holds: only the fastener
			// places them.
			var tab = MakePlate("tab", at: new Point3D(0, 600, 0), halfSize: 100);
			var cleat = MakePlate("cleat", at: new Point3D(0, 800, 0), halfSize: 100);
			var bolts = MakeFastener("bolts", new Point3D(0, 450, 0), gusset, tab, cleat);
			var tabWeld = new Weld("tab-weld", tab, cleat);

			var data = new SorterData
			{
				Members = new List<Member>(frame),
				Plates = new List<Plate> { gusset, tab, cleat },
				Welds = new List<Weld> { new Weld("beam-weld", frame[1], gusset), tabWeld },
				Fasteners = new List<FastenerGrid> { bolts },
			};

			var joint = new ItemsSorter().Sort(data, ModelCoordinatorSettings).Joints.Single();

			joint.Plates.Should().Contain(new[] { tab, cleat });
			joint.Welds.Should().Contain(tabWeld);
		}

		/// <summary>
		/// A joint reaches for a plate from every node it was built from, so it is measured from the nearest of them
		/// too. The cleat is welded to a beam both joints hold and lies within the reach of both; it sits beside the end
		/// of the brace that framed into the first joint, while the node that joint was built around is farther from
		/// it than the second joint is.
		/// </summary>
		[Test]
		public void PlateBothJointsReach_GoesToTheJointWithTheNearestNode()
		{
			var nearColumn = MakeMember("near-column", from: new Point3D(0, 0, -3000), to: Origin, cssWidth: 300, cssHeight: 400);
			var beam = MakeMember("beam", from: Origin, to: new Point3D(1600, 0, 0), cssWidth: 200, cssHeight: 500);
			var farColumn = MakeMember("far-column", from: new Point3D(1600, 0, -3000), to: new Point3D(1600, 0, 0), cssWidth: 300, cssHeight: 400);
			// Ends inside the beam's node box at the origin, 350 mm along the beam.
			var brace = MakeMember("brace", from: new Point3D(350, 0, -150), to: new Point3D(350, -2000, -2150), cssWidth: 100, cssHeight: 100);
			// 950 mm from the origin and 650 mm from the far column, but about 610 mm from the brace's end.
			var cleat = MakePlate("cleat", at: new Point3D(950, 0, 0), halfSize: 50);

			var data = new SorterData
			{
				Members = new List<Member> { nearColumn, beam, farColumn, brace },
				Plates = new List<Plate> { cleat },
				Welds = new List<Weld> { new Weld("cleat-weld", beam, cleat) },
				Fasteners = new List<FastenerGrid>(),
			};

			var result = new ItemsSorter().Sort(data, ModelCoordinatorSettings);

			result.Joints.Should().HaveCount(2);
			result.Joints.Single(j => j.Plates.Contains(cleat)).Members.Should().Contain(brace);
		}

		/// <summary>
		/// A node box is clamped to the maximum inflate extent only when it inflates, and the girder's end connects
		/// nothing of its own, so its box is still the one its deep section sizes. The reach it lends the joint must be
		/// clamped like every other, or a cleat welded far along the girder lands in a joint it has nothing to do with.
		/// </summary>
		[Test]
		public void PlateWeldedFarAlongADeepMember_IsNotRecoveredThroughAnEndThatNeverInflated()
		{
			var column = MakeMember("column", from: new Point3D(0, 0, -3000), to: Origin, cssWidth: 300, cssHeight: 400);
			var beam = MakeMember("beam", from: Origin, to: new Point3D(0, -6000, 0), cssWidth: 200, cssHeight: 500);
			// Its end lies inside the beam's node box, but that node lies 200 mm across the girder, outside the end's own
			// box, so the end's box takes nothing.
			var girder = MakeMember("girder", from: new Point3D(0, 200, 0), to: new Point3D(8000, 200, 0), cssWidth: 300, cssHeight: 1000);
			// Within three times the girder end's own box, beyond three times any clamped one.
			var cleat = MakePlate("cleat", at: new Point3D(1500, 200, 0), halfSize: 50);

			var data = new SorterData
			{
				Members = new List<Member> { column, beam, girder },
				Plates = new List<Plate> { cleat },
				Welds = new List<Weld> { new Weld("cleat-weld", girder, cleat) },
				Fasteners = new List<FastenerGrid>(),
			};

			var joint = new ItemsSorter().Sort(data, ModelCoordinatorSettings).Joints.Single();

			joint.Members.Should().Contain(girder);
			joint.Plates.Should().NotContain(cleat);
		}

		private static readonly IPoint3D Origin = new Point3D(0, 0, 0);

		/// <summary>
		/// Sorts a column and a beam ending at the origin - enough for one joint to form - plus <paramref name="part"/>
		/// starting there. Both frame members are metres long, so neither can be read as detailing and the joint the
		/// part is judged at is always theirs.
		/// </summary>
		private static Member[] Frame() => new Member[]
		{
			MakeMember("column", from: new Point3D(0, 0, -7500), to: Origin, cssWidth: 300, cssHeight: 400),
			MakeMember("beam", from: Origin, to: new Point3D(8000, 0, 0), cssWidth: 200, cssHeight: 500),
		};

		private static SorterResult SortFrame(Member[] frame, Plate[] plates, FastenerGrid[] fasteners)
		{
			var members = new List<Member>(frame);

			var data = new SorterData
			{
				Members = members,
				Plates = new List<Plate>(plates),
				Welds = new List<Weld>(),
				Fasteners = new List<FastenerGrid>(fasteners),
			};

			return new ItemsSorter().Sort(data, ModelCoordinatorSettings);
		}

		private static Plate MakePlate(object parent, IPoint3D at, double halfSize)
		{
			var contour = new List<IPoint3D>
			{
				new Point3D(at.X, at.Y - halfSize, at.Z - halfSize),
				new Point3D(at.X, at.Y + halfSize, at.Z - halfSize),
				new Point3D(at.X, at.Y + halfSize, at.Z + halfSize),
				new Point3D(at.X, at.Y - halfSize, at.Z + halfSize),
			};

			return new Plate(parent, IdentityAt(at), contour, 20);
		}

		private static FastenerGrid MakeFastener(object parent, IPoint3D at, params Item[] clamping)
		{
			var grid = new FastenerGrid(parent, IdentityAt(at), new List<Point3D> { new Point3D(at.X, at.Y, at.Z) });
			grid.ClampedItems.AddRange(clamping);

			return grid;
		}

		private static Matrix44 IdentityAt(IPoint3D at)
			=> new Matrix44(at, new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1));

		private static SorterResult SortFrameWith(Member part)
		{
			var column = MakeMember("column", from: new Point3D(0, 0, -7500), to: Origin, cssWidth: 300, cssHeight: 400);
			var beam = MakeMember("beam", from: Origin, to: new Point3D(8000, 0, 0), cssWidth: 200, cssHeight: 500);

			var data = new SorterData
			{
				Members = new List<Member> { column, beam, part },
				Plates = new List<Plate>(),
				Welds = new List<Weld>(),
				Fasteners = new List<FastenerGrid>(),
			};

			return new ItemsSorter().Sort(data, ModelCoordinatorSettings);
		}

		/// <summary>
		/// What the Model Coordinator branch of the Tekla plugin passes (PluginCommon's Program.cs). The node box these
		/// produce is what the shape rule exists to be independent of, so the tests run against them rather than the
		/// helper's own looser defaults.
		/// </summary>
		private static SorterSettings ModelCoordinatorSettings => new SorterSettings
		{
			EnlargeNodeXout = 0.9,
			EnlargeNodeXin = 0.9,
			EnlargeNodeY = 0.8,
			EnlargeNodeZ = 0.8,
			LengthTolerance = 0.0005,
			PlateThicknessMult4Tolerance = 2,
			MaxInflateExtent = 230,
			JoinContinuousMemberByNodeBox = true,
		};

		/// <summary>
		/// <paramref name="cssWidth"/> and <paramref name="cssHeight"/> are the full section dimensions, the way
		/// <c>BulkSelectionHelper</c> hands them over: the rect spans the section and is centred on the reference line.
		/// </summary>
		private static Member MakeMember(object parent, IPoint3D from, IPoint3D to, double cssWidth, double cssHeight)
		{
			var axisX = new Vector3D(to.X - from.X, to.Y - from.Y, to.Z - from.Z).Normalize;
			var axisY = PerpendicularTo(axisX);
			var cssBounds = new Rect(
				new System.Windows.Point(-0.5 * cssWidth, -0.5 * cssHeight),
				new System.Windows.Point(0.5 * cssWidth, 0.5 * cssHeight));

			return new Member(parent, new Matrix44(from, axisX, axisY, (axisX * axisY).Normalize), from, to, cssBounds);
		}

		private static Vector3D PerpendicularTo(Vector3D axis)
		{
			// '|' is the dot product on Vector3D; '*' is the cross. Pick the reference axis the member is least
			// aligned with, or the cross product degenerates.
			var reference = Math.Abs(axis | new Vector3D(0, 0, 1)) > 0.9 ? new Vector3D(1, 0, 0) : new Vector3D(0, 0, 1);

			return (axis * reference).Normalize;
		}

		private sealed class ParentIdentityComparer : IEqualityComparer<Item>
		{
			// Dereferenced, not null-conditional: both comparers the sorter runs against in production do the same
			// (Item.ParentEqualityComparer, and the Tekla link's ItemEqualityComparer), so a null reaching a comparison
			// is a defect here too. Softening it here would hide exactly that.
			public bool Equals(Item x, Item y) => x.Parent.Equals(y.Parent);

			public int GetHashCode(Item obj) => obj.Parent.GetHashCode();
		}
	}
}
