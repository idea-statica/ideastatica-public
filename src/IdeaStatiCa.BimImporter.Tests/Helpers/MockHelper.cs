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

		public static IIdeaElement1D CreateElement(string name, IIdeaSegment3D segment, IIdeaCrossSection cssStart,
			IIdeaCrossSection cssEnd = null, IdeaVector3D eccentricityBegin = null, IdeaVector3D eccentricityEnd = null,
			double rotation = 0, string id = null)
		{
			id = MakeId(id);

			cssEnd = cssEnd ?? cssStart;
			eccentricityBegin = eccentricityBegin ?? new IdeaVector3D(0, 0, 0);
			eccentricityEnd = eccentricityEnd ?? new IdeaVector3D(0, 0, 0);

			IIdeaElement1D element = Substitute.For<IIdeaElement1D>();
			element.Id.Returns(id);
			element.Name.Returns(name);
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

			IdeaVector3D vector = new IdeaVector3D(x, y, z);
			node.Vector.Returns(vector);

			return node;
		}
	}
}