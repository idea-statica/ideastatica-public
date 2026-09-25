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
	/// What <see cref="MemberEndMiss"/> reports for an end of a member no joint took. Millimetres, under the settings
	/// the Model Coordinator branch of the Tekla plugin passes.
	/// </summary>
	public class MemberEndMissTest
	{
		private const double Tolerance = 1e-6;

		/// <summary>
		/// <see cref="Item.CustomComparer"/> is process-wide and <c>BulkSelectionHelper.FindJoints</c> replaces it with one
		/// that reads a Tekla identifier off every parent, under which these plain parents would never equal themselves.
		/// </summary>
		[SetUp]
		public void CompareItemsByTheirParent() => Item.CustomComparer = new ParentIdentityComparer();

		/// <summary>
		/// A post standing on the flange of a beam: its foot is 150 mm off the beam's axis, beyond the band of 0.8 times
		/// the half-depth, while the post's own node box reaches 0.9 times its section along its axis.
		/// </summary>
		[Test]
		public void PostOnTheFlange_LiesOutsideTheBeamsBand_WhileTheBeamCrossesItsNodeBox()
		{
			var beam = MakeMember("beam", from: new Point3D(0, 0, 0), to: new Point3D(8000, 0, 0));
			var post = MakeMember("post", from: new Point3D(4000, 0, 150), to: new Point3D(4000, 0, 3594));

			var foot = MeasureFoot(post, beam);

			foot.Nearest.Should().BeSameAs(beam);
			foot.RelativePosition.Should().BeApproximately(0.5, Tolerance);
			Math.Abs(foot.PositionFromAxis.Y).Should().BeApproximately(150, Tolerance);
			foot.Band.Bottom.Should().BeApproximately(92, Tolerance);
			foot.Band.Contains(foot.PositionFromAxis).Should().BeFalse();
			foot.NodeBoxOverflow.Should().BeApproximately(0, Tolerance);
		}

		/// <summary>
		/// The band is measured from the LCS axis. With the LCS origin on the bottom flange instead of the centre line the
		/// foot lies 265 mm from the axis, and only the centre-line offset still says it is 150 mm from the part's middle.
		/// </summary>
		[Test]
		public void LcsOriginOffTheCentreLine_ShowsInTheCentreLineOffset()
		{
			var beam = MakeMember("beam", from: new Point3D(0, 0, 0), to: new Point3D(8000, 0, 0), lcsOrigin: new Point3D(0, 0, -115));
			var post = MakeMember("post", from: new Point3D(4000, 0, 150), to: new Point3D(4000, 0, 3594));

			var foot = MeasureFoot(post, beam);

			Math.Abs(foot.PositionFromAxis.Y).Should().BeApproximately(265, Tolerance);
			Math.Abs(foot.PositionFromCentreLine.Y).Should().BeApproximately(150, Tolerance);
		}

		/// <summary>
		/// The beam's axis lies 150 mm past the post's end, where the node box reaches 0.5 x 240 = 120 mm by
		/// <see cref="SorterSettings.EnlargeNodeXout"/> - and would reach 384 mm by the inward
		/// <see cref="SorterSettings.EnlargeNodeXin"/>. Measured at a begin and at an end, whose axes point opposite ways.
		/// </summary>
		[TestCase(150, 3594, true)]
		[TestCase(3594, 150, false)]
		public void NodeBox_ReachesPastTheEndOfTheMemberByXout(double fromZ, double toZ, bool footIsBegin)
		{
			var beam = MakeMember("beam", from: new Point3D(0, 0, 0), to: new Point3D(8000, 0, 0));
			var post = MakeMember("post", from: new Point3D(4000, 0, fromZ), to: new Point3D(4000, 0, toZ));
			var settings = ModelCoordinatorSettings;
			settings.EnlargeNodeXin = 1.6;
			settings.EnlargeNodeXout = 0.5;

			var foot = MemberEndMiss.Measure(new[] { post }, new[] { post, beam }, settings).Single(miss => miss.AtBegin == footIsBegin);

			foot.NodeBoxOverflow.Should().BeApproximately(30, Tolerance);
		}

		[Test]
		public void MemberWithNoOther_HasNoNearest()
		{
			var post = MakeMember("post", from: new Point3D(4000, 0, 150), to: new Point3D(4000, 0, 3594));

			var misses = MemberEndMiss.Measure(new[] { post }, new[] { post }, ModelCoordinatorSettings);

			misses.Should().HaveCount(2).And.OnlyContain(miss => miss.Nearest == null);
		}

		private static MemberEndMiss MeasureFoot(Member post, Member beam)
			=> MemberEndMiss.Measure(new[] { post }, new[] { post, beam }, ModelCoordinatorSettings).Single(miss => miss.AtBegin);

		/// <summary>What the Model Coordinator branch of the Tekla plugin passes (PluginCommon's Program.cs).</summary>
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
		/// An HEA240's outline - 240 wide, 230 deep - with the LCS on the centre line unless <paramref name="lcsOrigin"/>
		/// moves it.
		/// </summary>
		private static Member MakeMember(object parent, IPoint3D from, IPoint3D to, IPoint3D lcsOrigin = null)
		{
			var axisX = new Vector3D(to.X - from.X, to.Y - from.Y, to.Z - from.Z).Normalize;
			// '|' is the dot product on Vector3D and '*' the cross; the reference axis is the one the member is least
			// aligned with, or the cross product degenerates.
			var reference = Math.Abs(axisX | new Vector3D(0, 0, 1)) > 0.9 ? new Vector3D(1, 0, 0) : new Vector3D(0, 0, 1);
			var axisY = (axisX * reference).Normalize;
			var cssBounds = new Rect(new System.Windows.Point(-120, -115), new System.Windows.Point(120, 115));

			return new Member(parent, new Matrix44(lcsOrigin ?? from, axisX, axisY, (axisX * axisY).Normalize), from, to, cssBounds);
		}

		private sealed class ParentIdentityComparer : IEqualityComparer<Item>
		{
			public bool Equals(Item x, Item y) => x.Parent.Equals(y.Parent);

			public int GetHashCode(Item obj) => obj.Parent.GetHashCode();
		}
	}
}
