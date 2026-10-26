using FluentAssertions;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Geometry;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace IdeaStatiCa.RamToIdeaTest.Geometry
{
	[TestFixture]
	public class LineTest
	{
		[Test]
		public void Vector_PointsFromStartToEnd()
		{
			// Setup
			RamNode start = new RamNode(1, 1, 1);
			RamNode end = new RamNode(-2, 5, 3);

			// Tested method
			Line line = new Line(1, start, end, false);

			// Assert
			line.Vector.Should().Be(new Vector3D(-3, 4, 2));
		}

		[Test]
		public void AddIntermediateNode_WhenAllowsIntermediateNodeIsFalse_ThrowsInvalidOperationException()
		{
			// Setup
			RamNode start = new RamNode(1, 1, 1);
			RamNode end = new RamNode(-2, 5, 3);

			// Tested method
			Line line = new Line(1, start, end, false);

			// Assert
			line.Vector.Should().Be(new Vector3D(-3, 4, 2));
		}
	}
}