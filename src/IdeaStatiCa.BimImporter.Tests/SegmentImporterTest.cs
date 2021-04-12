using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Tests.Helpers;
using NSubstitute;
using NUnit.Framework;
using System;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class SegmentImporterTest
	{
		private static Vector3D CreateVector(double x, double y, double z)
		{
			return new Vector3D()
			{
				X = x,
				Y = y,
				Z = z,
			};
		}

		private static IIdeaLineSegment3D CreateMockLineSegment()
		{
			IIdeaNode node1 = MockHelper.CreateNode(0, 0, 0);
			IIdeaNode node2 = MockHelper.CreateNode(1, 1, 1);

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

		//----------

		[Test]
		public void SegmentImport_WhenStartAndEndNodesAreTheSame_ThrowsException()
		{
			// Setup - StartNode and EndNode are the same instance
			IIdeaLineSegment3D segment = CreateMockLineSegment();
			IIdeaNode node = MockHelper.CreateNode(0, 0, 0);
			segment.StartNode.Returns(node);
			segment.EndNode.Returns(node);

			IImportContext ctx = Substitute.For<IImportContext>();
			SegmentImporter segmentImporter = new SegmentImporter();

			// Tested method
			Assert.That(() => segmentImporter.Import(ctx, segment), Throws.TypeOf<ConstraintException>());
		}

		[Test]
		public void SegmentImport_WhenLCSIsNotInstanceOfCoordSystemByVector_ThrowsException()
		{
			// Setup - set LocalCoordinateSystem to something else than CoordSystemByVector (CoordSystemByZup here)
			IIdeaLineSegment3D segment = CreateMockLineSegment();
			segment.LocalCoordinateSystem.Returns(new CoordSystemByZup());

			IImportContext ctx = Substitute.For<IImportContext>();
			SegmentImporter segmentImporter = new SegmentImporter();

			// Tested method
			Assert.That(() => segmentImporter.Import(ctx, segment), Throws.TypeOf<NotImplementedException>());
		}

		[Test]
		public void SegmentImport_WhenSegmentIsNotLineOrArcSegment_ThrowsException()
		{
			// Setup
			IIdeaNode node1 = MockHelper.CreateNode(0, 0, 0);
			IIdeaNode node2 = MockHelper.CreateNode(1, 1, 1);

			IIdeaSegment3D segment = Substitute.For<IIdeaSegment3D>();
			segment.Id.Returns("1");
			segment.StartNode.Returns(node1);
			segment.EndNode.Returns(node2);
			segment.LocalCoordinateSystem.Returns(new CoordSystemByVector());

			IImportContext ctx = Substitute.For<IImportContext>();
			SegmentImporter segmentImporter = new SegmentImporter();

			// Tested method
			Assert.That(() => segmentImporter.Import(ctx, segment), Throws.TypeOf<NotImplementedException>());
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

			IImportContext ctx = Substitute.For<IImportContext>();
			SegmentImporter segmentImporter = new SegmentImporter();

			// Tested method
			Assert.That(() => segmentImporter.Import(ctx, segment), Throws.TypeOf<ConstraintException>());
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

			IImportContext ctx = Substitute.For<IImportContext>();
			SegmentImporter segmentImporter = new SegmentImporter();

			// Tested method
			Segment3D iomSegment = (Segment3D)segmentImporter.Import(ctx, segment);
			CoordSystemByVector lcs = iomSegment.LocalCoordinateSystem as CoordSystemByVector;

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
	}
}