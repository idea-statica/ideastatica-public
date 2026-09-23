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
