using CI.Geometry3D;
using FluentAssertions;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.TeklaStructuresTest
{
	public class PlateContourTest
	{
		/// <summary>
		/// The four corners of a 12 mm stiffener whose Tekla face loop walks the contour twice,
		/// taken from a hub import dump, in millimetres. The corner opposite the origin is
		/// reported as two distinct points 0.036 mm apart - <c>C1</c> on the first traversal,
		/// <c>C2</c> on the second - while A, B and D repeat bit-exactly.
		/// </summary>
		private static readonly Dictionary<string, IPoint3D> Corners = new Dictionary<string, IPoint3D>
		{
			["A"] = new Point3D(-3.0224783622908931E-11, 3.5029618830954734E-11, 0.0),
			["B"] = new Point3D(0.0080533706975985386, 169.99993896414645, 0.0),
			["C1"] = new Point3D(220.50875854430468, 169.98001098487539, 0.0),
			["C2"] = new Point3D(220.50033569273467, 170.01535034034285, 0.0),
			["D"] = new Point3D(220.50076293885876, -1.6098955047716714E-09, 0.0),
		};

		private const string DoubledLoop = "A,B,C1,D,A,B,C2,D";

		[TestCase(0, "A,B,C1,D")]
		[TestCase(1, "B,C1,D,A")]
		[TestCase(2, "D,A,B,C2")]
		[TestCase(3, "D,A,B,C2")]
		[TestCase(4, "A,B,C2,D")]
		[TestCase(5, "B,C2,D,A")]
		[TestCase(6, "D,A,B,C1")]
		[TestCase(7, "D,A,B,C1")]
		public void DoubledLoopKeepsOneTraversalWhicheverVertexItStartsFrom(int rotation, string expected)
		{
			IReadOnlyList<IPoint3D> loop = Rotate(Contour(DoubledLoop), rotation);

			var result = PlateContour.FirstClosedContour(loop);

			Describe(result).Should().Be(expected);
		}

		[Test]
		public void DoubledTriangleKeepsOneTraversal()
		{
			var result = PlateContour.FirstClosedContour(Contour("A,B,C1,A,B,C1"));

			Describe(result).Should().Be("A,B,C1");
		}

		/// <summary>
		/// A repeat two vertices apart cannot bound a region, so collapsing onto it would leave a
		/// contour of a single point.
		/// </summary>
		[Test]
		public void RepeatTooCloseToCloseACycleIsKept()
		{
			var loop = Contour("A,A,B,C1,D");

			var result = PlateContour.FirstClosedContour(loop);

			result.Should().BeSameAs(loop);
		}

		[Test]
		public void LoopVisitingEveryVertexOnceIsUnchanged()
		{
			var loop = Contour("A,B,C1,D");

			var result = PlateContour.FirstClosedContour(loop);

			result.Should().BeSameAs(loop);
		}

		/// <summary>
		/// The repeats seen in the field are bit-exact, but nothing in the Tekla API promises that,
		/// so these two pin what the coincidence tolerance actually accepts and rejects.
		/// </summary>
		[Test]
		public void RepeatWithinToleranceClosesTheContour()
		{
			var loop = Contour("A,B,C1,D").Append(Nudged("A", 0.00005)).ToList();

			var result = PlateContour.FirstClosedContour(loop);

			Describe(result).Should().Be("A,B,C1,D");
		}

		[Test]
		public void RepeatOutsideToleranceDoesNotCloseTheContour()
		{
			var loop = Contour("A,B,C1,D").Append(Nudged("A", 0.0002)).ToList();

			var result = PlateContour.FirstClosedContour(loop);

			result.Should().BeSameAs(loop);
		}

		/// <summary>
		/// The range primitive callers use when their vertices are not <see cref="IPoint3D"/> and they
		/// have to slice their own list.
		/// </summary>
		[TestCase(0, 0, 4)]
		[TestCase(2, 1, 4)]
		public void RangeLocatesTheSameContourTheListOverloadReturns(int rotation, int expectedStart, int expectedCount)
		{
			IReadOnlyList<IPoint3D> loop = Rotate(Contour(DoubledLoop), rotation);

			PlateContour.FindFirstClosedContour(loop, out int start, out int count);

			start.Should().Be(expectedStart);
			count.Should().Be(expectedCount);
			Describe(loop.Skip(start).Take(count).ToList())
				.Should().Be(Describe(PlateContour.FirstClosedContour(loop)));
		}

		[Test]
		public void RangeSpansEverythingWhenNothingRepeats()
		{
			IReadOnlyList<IPoint3D> loop = Contour("A,B,C1,D");

			PlateContour.FindFirstClosedContour(loop, out int start, out int count);

			start.Should().Be(0);
			count.Should().Be(4);
		}

		private static IPoint3D Nudged(string name, double byMm)
		{
			IPoint3D origin = Corners[name];
			return new Point3D(origin.X + byMm, origin.Y, origin.Z);
		}

		private static IReadOnlyList<IPoint3D> Contour(string vertexNames)
		{
			return vertexNames.Split(',').Select(name => Corners[name]).ToList();
		}

		private static IReadOnlyList<IPoint3D> Rotate(IReadOnlyList<IPoint3D> loop, int by)
		{
			return Enumerable.Range(0, loop.Count).Select(i => loop[(i + by) % loop.Count]).ToList();
		}

		private static string Describe(IReadOnlyList<IPoint3D> contour)
		{
			return string.Join(",", contour.Select(point => Corners.First(corner => corner.Value == point).Key));
		}
	}
}
