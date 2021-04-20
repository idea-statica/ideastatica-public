using IdeaStatiCa.BimApi;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IdeaStatiCa.BimImporter.Tests
{
    [TestFixture]
    public class GeometryTest
    {
        private class GeometryBuilder
        {
            private static readonly Regex Regexp = new Regex(@"(\w+)\((.+?)\)");

            public Dictionary<int, IIdeaMember1D> Members { get; } = new Dictionary<int, IIdeaMember1D>();
            public Dictionary<int, IIdeaNode> Nodes { get; } = new Dictionary<int, IIdeaNode>();

            private int _elementCount = 0;

            public IIdeaModel GetModel()
            {
                IIdeaModel model = Substitute.For<IIdeaModel>();
                model.GetMembers().Returns(Members.Values.ToHashSet());
                return model;
            }

            public GeometryBuilder Member(int i, string config)
            {
                IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
                member.Id.Returns($"member{i}");
                Members[i] = member;

                List<IIdeaElement1D> elements = new List<IIdeaElement1D>();
                member.Elements.Returns(elements);

                foreach (Match match in Regexp.Matches(config))
                {
                    IIdeaElement1D element = Substitute.For<IIdeaElement1D>();
                    element.Id.Returns($"element{_elementCount}");
                    _elementCount++;

                    elements.Add(element);

                    IIdeaSegment3D segment = CreateSegment(match.Groups[1].Value,
                        Regex.Matches(match.Groups[2].Value, @"\d+")
                        .Cast<Match>()
                        .Select(x => int.Parse(x.Value))
                        .ToArray());

                    element.Segment.Returns(segment);
                }

                return this;
            }

            private IIdeaSegment3D CreateSegment(string type, int[] args)
            {
                var nodes = args.Select(x => GetNode(x)).ToArray();

                switch (type.ToLower())
                {
                    case "line":
                        IIdeaLineSegment3D line = Substitute.For<IIdeaLineSegment3D>();
                        line.StartNode.Returns(nodes[0]);
                        line.EndNode.Returns(nodes[1]);
                        return line;

                    case "arc":
                        IIdeaArcSegment3D arc = Substitute.For<IIdeaArcSegment3D>();
                        arc.StartNode.Returns(nodes[0]);
                        arc.ArcPoint.Returns(nodes[1]);
                        arc.EndNode.Returns(nodes[2]);
                        return arc;
                }

                throw new NotImplementedException();
            }

            private IIdeaNode GetNode(int id)
            {
                if (Nodes.TryGetValue(id, out IIdeaNode node))
                {
                    return node;
                }

                node = Substitute.For<IIdeaNode>();
                node.Id.Returns($"node{id}");
                Nodes.Add(id, node);

                return node;
            }
        }

        [Test]
        public void GetConnectedMembers_WhenArgumentIsUnknownNode_ReturnsEmptyEnumerable()
        {
            // Setup
            IIdeaModel model = Substitute.For<IIdeaModel>();
            model.GetMembers().Returns(new HashSet<IIdeaMember1D>());

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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

            Geometry geometry = new Geometry();
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
    }
}