using IdeaStatiCa.BimApi;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IdeaStatiCa.BimImporter.Tests.Helpers
{
    internal class GeometryBuilder
    {
        private static readonly Regex Regexp = new Regex(@"(\w+)\((.+?)\)");

        public Dictionary<int, IIdeaMember1D> Members { get; } = new Dictionary<int, IIdeaMember1D>();
        public Dictionary<int, IIdeaNode> Nodes { get; } = new Dictionary<int, IIdeaNode>();

        private int _elementCount = 0;

        public IIdeaModel GetModel()
        {
            IIdeaModel model = Substitute.For<IIdeaModel>();
            model.GetMembers().Returns(Members.Values.ToHashSet());
			model.GetOriginSettings().Returns(new IdeaRS.OpenModel.OriginSettings() { CountryCode = IdeaRS.OpenModel.CountryCode.ECEN });
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
}