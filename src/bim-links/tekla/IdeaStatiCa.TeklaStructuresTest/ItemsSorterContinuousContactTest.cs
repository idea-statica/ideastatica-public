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
	/// When a member that ends on the face of a passing member joins it. Millimetres, under the settings the Model
	/// Coordinator branch of the Tekla plugin passes: a band of 0.8 times the half-section, a box reaching 0.9 times the
	/// section along the ending member.
	/// </summary>
	public class ItemsSorterContinuousContactTest
	{
		[SetUp]
		public void CompareItemsByTheirParent() => Item.CustomComparer = new ParentIdentityComparer();

		/// <summary>
		/// The post's foot is 150 mm off the beam's axis, past the 92 mm band, while the post's own box reaches 216 mm
		/// along it.
		/// </summary>
		[Test]
		public void PostStandingOnTheFlange_JoinsTheBeam()
		{
			var beam = MakeMember("beam", from: new Point3D(0, 0, 0), to: new Point3D(8000, 0, 0), cssWidth: 240, cssHeight: 230);
			var post = MakeMember("post", from: new Point3D(4000, 0, 150), to: new Point3D(4000, 0, 3594), cssWidth: 240, cssHeight: 230);

			var joint = Sort(Settings(joinByNodeBox: true), beam, post).Joints.Single();

			joint.Members.Should().HaveCount(2).And.Contain(beam).And.Contain(post);
		}

		[Test]
		public void PostStandingOnTheFlange_StaysUnjoined_WhenOnlyTheBandDecides()
		{
			var beam = MakeMember("beam", from: new Point3D(0, 0, 0), to: new Point3D(8000, 0, 0), cssWidth: 240, cssHeight: 230);
			var post = MakeMember("post", from: new Point3D(4000, 0, 150), to: new Point3D(4000, 0, 3594), cssWidth: 240, cssHeight: 230);

			Sort(Settings(joinByNodeBox: false), beam, post).Joints.Should().BeEmpty();
		}

		/// <summary>
		/// A bracket welded to a column's flange: its end lies 115 mm off the column's axis - the column's half-depth -
		/// against a 92 mm band, and its box reaches 270 mm past its end.
		/// </summary>
		[Test]
		public void BracketEndingOnTheColumnFace_JoinsTheColumn()
		{
			var column = MakeMember("column", from: new Point3D(0, 0, 0), to: new Point3D(0, 0, 3444), cssWidth: 240, cssHeight: 230);
			var bracket = MakeMember("bracket", from: new Point3D(710, 0, 149), to: new Point3D(115, 0, 149), cssWidth: 150, cssHeight: 300);

			var joint = Sort(Settings(joinByNodeBox: true), column, bracket).Joints.Single();

			joint.Members.Should().HaveCount(2).And.Contain(column).And.Contain(bracket);
		}

		/// <summary>
		/// A rail 15 mm clear of the post's face touches neither the post nor the beam. Joining the beam grows the post's
		/// box until the rail's centre line lies inside it; the box the post started with does not reach it.
		/// </summary>
		[Test]
		public void MemberPassingClearOfTheNode_DoesNotJoinThroughTheBoxAnotherJoinGrew()
		{
			var beam = MakeMember("beam", from: new Point3D(0, 0, 0), to: new Point3D(8000, 0, 0), cssWidth: 240, cssHeight: 230);
			var post = MakeMember("post", from: new Point3D(4000, 0, 150), to: new Point3D(4000, 0, 3594), cssWidth: 240, cssHeight: 230);
			var rail = MakeMember("rail", from: new Point3D(4180, -2000, 250), to: new Point3D(4180, 2000, 250), cssWidth: 100, cssHeight: 100);

			var joint = Sort(Settings(joinByNodeBox: true), beam, post, rail).Joints.Single();

			joint.Members.Should().HaveCount(2).And.Contain(beam).And.Contain(post);
		}

		/// <summary>The post's box reaches 216 mm along it; a beam 300 mm below its foot is out of reach.</summary>
		[Test]
		public void PostEndingBeyondTheReachOfItsBox_StaysUnjoined()
		{
			var beam = MakeMember("beam", from: new Point3D(0, 0, 0), to: new Point3D(8000, 0, 0), cssWidth: 240, cssHeight: 230);
			var post = MakeMember("post", from: new Point3D(4000, 0, 300), to: new Point3D(4000, 0, 3744), cssWidth: 240, cssHeight: 230);

			Sort(Settings(joinByNodeBox: true), beam, post).Joints.Should().BeEmpty();
		}

		private static SorterResult Sort(SorterSettings settings, params Member[] members)
		{
			var data = new SorterData
			{
				Members = new List<Member>(members),
				Plates = new List<Plate>(),
				Welds = new List<Weld>(),
				Fasteners = new List<FastenerGrid>(),
			};

			return new ItemsSorter().Sort(data, settings);
		}

		private static SorterSettings Settings(bool joinByNodeBox) => new SorterSettings
		{
			EnlargeNodeXout = 0.9,
			EnlargeNodeXin = 0.9,
			EnlargeNodeY = 0.8,
			EnlargeNodeZ = 0.8,
			LengthTolerance = 0.0005,
			PlateThicknessMult4Tolerance = 2,
			MaxInflateExtent = 230,
			JoinContinuousMemberByNodeBox = joinByNodeBox,
		};

		private static Member MakeMember(object parent, IPoint3D from, IPoint3D to, double cssWidth, double cssHeight)
		{
			var axisX = new Vector3D(to.X - from.X, to.Y - from.Y, to.Z - from.Z).Normalize;
			// '|' is the dot product on Vector3D and '*' the cross; the reference axis is the one the member is least
			// aligned with, or the cross product degenerates.
			var reference = Math.Abs(axisX | new Vector3D(0, 0, 1)) > 0.9 ? new Vector3D(1, 0, 0) : new Vector3D(0, 0, 1);
			var axisY = (axisX * reference).Normalize;
			var cssBounds = new Rect(
				new System.Windows.Point(-0.5 * cssWidth, -0.5 * cssHeight),
				new System.Windows.Point(0.5 * cssWidth, 0.5 * cssHeight));

			return new Member(parent, new Matrix44(from, axisX, axisY, (axisX * axisY).Normalize), from, to, cssBounds);
		}

		private sealed class ParentIdentityComparer : IEqualityComparer<Item>
		{
			public bool Equals(Item x, Item y) => x.Parent.Equals(y.Parent);

			public int GetHashCode(Item obj) => obj.Parent.GetHashCode();
		}
	}
}
