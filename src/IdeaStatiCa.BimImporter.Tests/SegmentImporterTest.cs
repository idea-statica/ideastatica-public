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

		private static IIdeaLineSegment3D CreateMockLineSegment()
		{
			IIdeaNode node1 = CreateMockNode("node1", 0, 0, 0);
			IIdeaNode node2 = CreateMockNode("node2", 1, 1, 1);

			IIdeaLineSegment3D segment = Substitute.For<IIdeaLineSegment3D>();
			segment.Id.Returns("1");
			segment.StartNode.Returns(node1);
			segment.EndNode.Returns(node2);
			segment.LocalCoordinateSystem.Returns(new CoordSystemByVector());
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
	}
}