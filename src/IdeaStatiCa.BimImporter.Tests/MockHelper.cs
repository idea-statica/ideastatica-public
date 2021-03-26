using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using NSubstitute;
using System;

namespace IdeaStatiCa.BimImporter.Tests
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

		public static IdeaVector3D CreateVector3D(float x, float y, float z)
		{
			return new IdeaVector3D()
			{
				X = x,
				Y = y,
				Z = z
			};
		}

		public static IIdeaElement1D CreateElement(string name, IIdeaNode startNode, IIdeaNode endNode, IIdeaCrossSection cssStart,
			IIdeaCrossSection cssEnd, IdeaVector3D eccentricityBegin, IdeaVector3D eccentricityEnd, double rotation, IIdeaSegment3D segment,
			string id = null)
		{
			id = MakeId(id);

			IIdeaElement1D element = Substitute.For<IIdeaElement1D>();
			element.Id.Returns(id);
			element.Name.Returns(name);
			element.StartNode.Returns(startNode);
			element.EndNode.Returns(endNode);
			element.StartCrossSection.Returns(cssStart);
			element.EndCrossSection.Returns(cssEnd);
			element.EccentricityBegin.Returns(eccentricityBegin);
			element.EccentricityEnd.Returns(eccentricityEnd);
			element.RotationRx.Returns(rotation);
			element.Segment.Returns(segment);

			return element;
		}

		public static IIdeaNode CreateNode(float x, float y, float z, string id = null)
		{
			id = MakeId(id);

			IIdeaNode node = Substitute.For<IIdeaNode>();
			node.Id.Returns(id);
			node.X.Returns(x);
			node.Y.Returns(y);
			node.Z.Returns(z);

			return node;
		}
	}
}