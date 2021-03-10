using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class SegmentImporterTest
	{
		private static IIdeaNode CreateMockNode(string id, float x, float y, float z)
		{
			IIdeaNode node = Substitute.For<IIdeaNode>();
			node.Id.Returns(id);
			node.X.Returns(x);
			node.Y.Returns(y);
			node.Z.Returns(z);
			return node;
		}

		private static Vector3D CreateVector(double x, double y, double z)
		{
			return new Vector3D()
			{
				X = x,
				Y = y,
				Z = z,
			};
		}

		//----------

		private static IIdeaLineSegment3D CreateMockLineSegment()
		{
			IIdeaNode node1 = CreateMockNode("node1", 0, 0, 0);
			IIdeaNode node2 = CreateMockNode("node2", 1, 1, 1);

			CoordSystemByVector coordSystem = new CoordSystemByVector()
			{
				VecX = CreateVector(1, 0, 0),
				VecY = CreateVector(0, 1, 0),
				VecZ = CreateVector(0, 0, 1)
			};

			IIdeaLineSegment3D segment = Substitute.For<IIdeaLineSegment3D>();
			segment.Id.Returns("1");
			segment.StartNode.Returns(node1);
			segment.EndNode.Returns(node2);
			segment.LocalCoordinateSystem.Returns(coordSystem);
			return segment;
		}

		[Test]
		public void SegmentImport_WhenStartAndEndNodesAreTheSame_ThrowsException()
		{
			// Setup - StartNode and EndNode are the same instance
			IIdeaLineSegment3D segment = CreateMockLineSegment();
			IIdeaNode node = CreateMockNode("node1", 0, 0, 0);
			segment.StartNode.Returns(node);
			segment.EndNode.Returns(node);

			SegmentImporter segmentImporter = new SegmentImporter(null);

			// Tested method
			Assert.That(() => segmentImporter.Import(new ImportContext(), segment), Throws.TypeOf<ConstraintException>());
		}

		[Test]
		public void SegmentImport_WhenLCSIsNotInstanceOfCoordSystemByVector_ThrowsException()
		{
			// Setup - set LocalCoordinateSystem to something else than CoordSystemByVector (CoordSystemByZup here)
			IIdeaLineSegment3D segment = CreateMockLineSegment();
			segment.LocalCoordinateSystem.Returns(new CoordSystemByZup());

			SegmentImporter segmentImporter = new SegmentImporter(null);

			// Tested method
			Assert.That(() => segmentImporter.Import(new ImportContext(), segment), Throws.TypeOf<ConstraintException>());
		}

		[Test]
		public void SegmentImport_WhenSegmentIsNotLineOrArcSegment_ThrowsException()
		{
			// Setup
			IIdeaNode node1 = CreateMockNode("node1", 0, 0, 0);
			IIdeaNode node2 = CreateMockNode("node2", 1, 1, 1);

			IIdeaSegment3D segment = Substitute.For<IIdeaSegment3D>();
			segment.Id.Returns("1");
			segment.StartNode.Returns(node1);
			segment.EndNode.Returns(node2);
			segment.LocalCoordinateSystem.Returns(new CoordSystemByVector());

			SegmentImporter segmentImporter = new SegmentImporter(null);

			// Tested method
			Assert.That(() => segmentImporter.Import(new ImportContext(), segment), Throws.TypeOf<ConstraintException>());
		}

		[TestCase(new double[] { 2, 0, 0 }, new double[] { 0, 1, 0 }, new double[] { 0, 0, 1 })]
		[TestCase(new double[] { 1, 0, 0 }, new double[] { 0, 2, 0 }, new double[] { 0, 0, 1 })]
		[TestCase(new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 }, new double[] { 0, 0, 2 })]
		public void SegmentImport_WhenLCSBasisVectorAreNotUnitVectors_ThrowsException(double[] vecX, double[] vecY, double[] vecZ)
		{
			// Setup
			CoordSystemByVector coordSystem = new CoordSystemByVector()
			{
				VecX = CreateVector(vecX[0], vecX[1], vecX[2]),
				VecY = CreateVector(vecY[0], vecY[1], vecY[2]),
				VecZ = CreateVector(vecZ[0], vecZ[1], vecZ[2])
			};

			IIdeaLineSegment3D segment = CreateMockLineSegment();
			segment.LocalCoordinateSystem.Returns(coordSystem);

			SegmentImporter segmentImporter = new SegmentImporter(Substitute.For<IImporter<IIdeaNode>>());

			// Tested method
			Assert.That(() => segmentImporter.Import(new ImportContext(), segment), Throws.TypeOf<ConstraintException>());
		}

		[Test]
		public void SegmentImport_ShouldNormalizeLCSVectors()
		{
			// Setup
			CoordSystemByVector coordSystem = new CoordSystemByVector()
			{
				VecX = CreateVector(1 + double.Epsilon, 0 + double.Epsilon, 0 - double.Epsilon),
				VecY = CreateVector(0 + double.Epsilon, 1 - double.Epsilon, 0 - double.Epsilon),
				VecZ = CreateVector(0 - double.Epsilon, 0 + double.Epsilon, 1 + double.Epsilon)
			};

			IIdeaLineSegment3D segment = CreateMockLineSegment();
			segment.LocalCoordinateSystem.Returns(coordSystem);

			SegmentImporter segmentImporter = new SegmentImporter(Substitute.For<IImporter<IIdeaNode>>());

			// Tested method
			ReferenceElement refElm = segmentImporter.Import(new ImportContext(), segment);
			CoordSystemByVector lcs = ((refElm.Element as Segment3D).LocalCoordinateSystem as CoordSystemByVector);

			// Assert
			Assert.That(lcs.VecX.X, Is.EqualTo(1));
			Assert.That(lcs.VecX.Y, Is.EqualTo(0));
			Assert.That(lcs.VecX.Z, Is.EqualTo(0));

			Assert.That(lcs.VecY.X, Is.EqualTo(0));
			Assert.That(lcs.VecY.Y, Is.EqualTo(1));
			Assert.That(lcs.VecY.Z, Is.EqualTo(0));

			Assert.That(lcs.VecZ.X, Is.EqualTo(0));
			Assert.That(lcs.VecZ.Y, Is.EqualTo(0));
			Assert.That(lcs.VecZ.Z, Is.EqualTo(1));
		}

		public void SegmentImport_ShouldAddTheLineSegmentIntoOpenModel()
		{
			// Setup
			IIdeaLineSegment3D segment = CreateMockLineSegment();
			SegmentImporter segmentImporter = new SegmentImporter(null);
			ImportContext ctx = new ImportContext();

			// Tested method
			ReferenceElement refElm = segmentImporter.Import(ctx, segment);

			// Assert
			ctx.OpenModel.LineSegment3D.Contains(refElm.Element as LineSegment3D);
		}
	}
}