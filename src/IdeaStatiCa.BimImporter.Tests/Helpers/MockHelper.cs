using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using NSubstitute;
using System;

namespace IdeaStatiCa.BimImporter.Tests.Helpers
{
	internal static class MockHelper
	{
		private static int IdCounter = 1;

		private static string MakeId(string id)
		{
			if (id != null)
			{
				return id;
			}

			return Guid.NewGuid().ToString();
		}

		public static ReferenceElement CreateRefElement(int? id = null)
		{
			return new ReferenceElement(new OpenElementId()
			{
				Id = id.GetValueOrDefault(IdCounter++)
			});
		}

		public static IIdeaElement1D CreateElement(string name, IIdeaSegment3D segment, double rotation = 0, string id = null)
		{
			id = MakeId(id);

			IIdeaElement1D element = Substitute.For<IIdeaElement1D>();
			element.Id.Returns(id);
			element.Name.Returns(name);
			element.RotationRx.Returns(rotation);
			element.Segment.Returns(segment);

			return element;
		}

		public static IIdeaElement1D CreateElement(string id = null)
		{
			id = MakeId(id);

			var segment = CreateLineSegment(CreateNode(), CreateNode());

			IIdeaElement1D element = Substitute.For<IIdeaElement1D>();
			element.Id.Returns(id);
			element.Name.Returns($"element-{id}");
			element.Segment.Returns(segment);

			return element;
		}

		public static IIdeaNode CreateNode(double x = 0.0, double y = 0.0, double z = 0.0, string id = null)
		{
			id = MakeId(id);

			IIdeaNode node = Substitute.For<IIdeaNode>();
			node.Id.Returns(id);

			IdeaVector3D vector = new IdeaVector3D(x, y, z);
			node.Vector.Returns(vector);

			return node;
		}

		public static IIdeaSegment3D CreateLineSegment(IIdeaNode start, IIdeaNode end, string name = null, string id = null)
		{
			id = MakeId(id);

			IIdeaSegment3D segment = Substitute.For<IIdeaSegment3D>();
			segment.Id.Returns(id);
			segment.Name.Returns(name ?? $"element-{id}");
			segment.StartNode.Returns(start);
			segment.EndNode.Returns(end);

			return segment;
		}
	}
}