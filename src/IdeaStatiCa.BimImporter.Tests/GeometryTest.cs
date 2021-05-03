using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Tests.Helpers;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class GeometryTest
	{
		[Test]
		public void GetConnectedMembers_WhenArgumentIsUnknownNode_ReturnsEmptyEnumerable()
		{
			// Setup
			IIdeaModel model = Substitute.For<IIdeaModel>();
			model.GetMembers().Returns(new HashSet<IIdeaMember1D>());

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(model);

			// Tested method
			IEnumerable<IIdeaMember1D> result = geometry.GetConnectedMembers(Substitute.For<IIdeaNode>());

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetNodesOnMember_WhenArgumentIsUnknownMember_ReturnsEmptyEnumerable()
		{
			// Setup
			IIdeaModel model = Substitute.For<IIdeaModel>();
			model.GetMembers().Returns(new HashSet<IIdeaMember1D>());

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(model);

			// Tested method
			IEnumerable<IIdeaNode> result = geometry.GetNodesOnMember(Substitute.For<IIdeaMember1D>());

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetNodesOnMember_OneMemberLineSegment()
		{
			// Setup: one member with a line segment
			GeometryBuilder builder = new GeometryBuilder();

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(builder.Member(1, "line(0,1)").GetModel());

			// Tested method
			IEnumerable<IIdeaNode> result = geometry.GetNodesOnMember(builder.Members[1]);

			// Assert
			Assert.That(result, Is.EquivalentTo(new List<IIdeaNode>() { builder.Nodes[0], builder.Nodes[1] }));
		}

		[Test]
		public void GetConnectedMembers_OneMemberLineSegment()
		{
			// Setup: one member with a line segment
			GeometryBuilder builder = new GeometryBuilder();

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(builder.Member(1, "line(0,1)").GetModel());

			// Tested method
			IEnumerable<IIdeaMember1D> result1 = geometry.GetConnectedMembers(builder.Nodes[0]);
			IEnumerable<IIdeaMember1D> result2 = geometry.GetConnectedMembers(builder.Nodes[1]);

			// Assert
			Assert.That(result1, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
			Assert.That(result2, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
		}

		[Test]
		public void GetNodesOnMember_OneMemberArcSegment()
		{
			// Setup: one member with an arc segment
			GeometryBuilder builder = new GeometryBuilder();

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(builder.Member(1, "arc(0,1,2)").GetModel());

			// Tested method
			IEnumerable<IIdeaNode> result = geometry.GetNodesOnMember(builder.Members[1]);

			// Assert
			Assert.That(result, Is.EquivalentTo(new List<IIdeaNode>() { builder.Nodes[0], builder.Nodes[1], builder.Nodes[2] }));
		}

		[Test]
		public void GetConnectedMembers_OneMemberArcSegment()
		{
			// Setup: one member with an arc segment
			GeometryBuilder builder = new GeometryBuilder();

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(builder.Member(1, "arc(0,1,2)").GetModel());

			// Tested method
			IEnumerable<IIdeaMember1D> result1 = geometry.GetConnectedMembers(builder.Nodes[0]);
			IEnumerable<IIdeaMember1D> result2 = geometry.GetConnectedMembers(builder.Nodes[1]);
			IEnumerable<IIdeaMember1D> result3 = geometry.GetConnectedMembers(builder.Nodes[2]);

			// Assert
			Assert.That(result1, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
			Assert.That(result2, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
			Assert.That(result3, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
		}

		[Test]
		public void GetConnectedMembers_OneMemberWithTwoLineSegments()
		{
			// Setup: one member with two line segments
			GeometryBuilder builder = new GeometryBuilder();

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(builder.Member(1, "line(0,1),line(1,2)").GetModel());

			// Tested method
			IEnumerable<IIdeaMember1D> result1 = geometry.GetConnectedMembers(builder.Nodes[0]);
			IEnumerable<IIdeaMember1D> result2 = geometry.GetConnectedMembers(builder.Nodes[1]);
			IEnumerable<IIdeaMember1D> result3 = geometry.GetConnectedMembers(builder.Nodes[2]);

			// Assert
			Assert.That(result1, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
			Assert.That(result2, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
			Assert.That(result3, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
		}

		[Test]
		public void GetConnectedMembers_TwoConnectedMembers()
		{
			// Setup: two connected members, each with their own line segment
			GeometryBuilder builder = new GeometryBuilder();

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(builder
				.Member(1, "line(0,1)")
				.Member(2, "line(1,2)")
				.GetModel());

			// Tested method
			IEnumerable<IIdeaMember1D> result1 = geometry.GetConnectedMembers(builder.Nodes[0]);
			IEnumerable<IIdeaMember1D> result2 = geometry.GetConnectedMembers(builder.Nodes[1]);
			IEnumerable<IIdeaMember1D> result3 = geometry.GetConnectedMembers(builder.Nodes[2]);

			// Assert
			Assert.That(result1, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
			Assert.That(result2, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1], builder.Members[2] }));
			Assert.That(result3, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[2] }));
		}

		[Test]
		public void GetNodesOnMember_TwoConnectedMembers()
		{
			// Setup: two connected members, each with their own line segment
			GeometryBuilder builder = new GeometryBuilder();

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(builder
				.Member(1, "line(0,1)")
				.Member(2, "line(1,2)")
				.GetModel());

			// Tested method
			IEnumerable<IIdeaNode> result1 = geometry.GetNodesOnMember(builder.Members[1]);
			IEnumerable<IIdeaNode> result2 = geometry.GetNodesOnMember(builder.Members[2]);

			// Assert
			Assert.That(result1, Is.EquivalentTo(new List<IIdeaNode>() { builder.Nodes[0], builder.Nodes[1] }));
			Assert.That(result2, Is.EquivalentTo(new List<IIdeaNode>() { builder.Nodes[1], builder.Nodes[2] }));
		}

		[Test]
		public void GetConnectedMembers_TwoInterceptingMembers()
		{
			// Setup: two members, each with two line segments forming a cross shaped connection
			GeometryBuilder builder = new GeometryBuilder();

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(builder
				.Member(1, "line(0,1),line(1,2)")
				.Member(2, "line(3,1),line(1,4)")
				.GetModel());

			// Tested method
			IEnumerable<IIdeaMember1D> result0 = geometry.GetConnectedMembers(builder.Nodes[0]);
			IEnumerable<IIdeaMember1D> result1 = geometry.GetConnectedMembers(builder.Nodes[1]);
			IEnumerable<IIdeaMember1D> result2 = geometry.GetConnectedMembers(builder.Nodes[2]);
			IEnumerable<IIdeaMember1D> result3 = geometry.GetConnectedMembers(builder.Nodes[3]);
			IEnumerable<IIdeaMember1D> result4 = geometry.GetConnectedMembers(builder.Nodes[4]);

			// Assert
			Assert.That(result0, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
			Assert.That(result1, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1], builder.Members[2] }));
			Assert.That(result2, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[1] }));
			Assert.That(result3, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[2] }));
			Assert.That(result4, Is.EquivalentTo(new List<IIdeaMember1D>() { builder.Members[2] }));
		}

		[Test]
		public void GetConnectedMembers_FourConnectingMembers()
		{
			// Setup: two members, each with two line segments forming a cross shaped connection
			GeometryBuilder builder = new GeometryBuilder();

			Geometry geometry = new Geometry(new NullLogger());
			geometry.Build(builder
				.Member(1, "line(0,1)")
				.Member(2, "line(1,2)")
				.Member(3, "line(3,1)")
				.Member(4, "line(4,1)")
				.GetModel());

			// Tested method
			IEnumerable<IIdeaMember1D> result = geometry.GetConnectedMembers(builder.Nodes[1]);

			// Assert
			Assert.That(result, Is.EquivalentTo(new List<IIdeaMember1D>() {
				builder.Members[1],
				builder.Members[2],
				builder.Members[3],
				builder.Members[4]
			}));
		}

		[Test]
		public void Build_WhenCalledTwiceWithTheSameModel_DoesntFail()
		{
			//Setup
			GeometryBuilder builder = new GeometryBuilder();
			IIdeaModel model = builder
				.Member(1, "line(0,1)")
				.Member(2, "line(1,2)")
				.Member(3, "line(3,1)")
				.Member(4, "line(4,1)")
				.GetModel();

			Geometry geometry = new Geometry(new NullLogger());

			// Tested method + assert for second call
			geometry.Build(model);
			Assert.That(() => geometry.Build(model), Throws.Nothing);

			// And sanity check for the data
			IEnumerable<IIdeaMember1D> result = geometry.GetConnectedMembers(builder.Nodes[1]);
			Assert.That(result, Is.EquivalentTo(new List<IIdeaMember1D>() {
				builder.Members[1],
				builder.Members[2],
				builder.Members[3],
				builder.Members[4]
			}));
		}
	}
}