using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Model;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	/// <inheritdoc cref="IIdeaMember1D"/>
	internal abstract class AbstractRamMember : IIdeaMember1D
	{
		public Member1DType Type
		{
			get
			{
				switch (MemberType)
				{
					case MemberType.Beam:
					case MemberType.VerticalBrace:
					case MemberType.HorizontalBrace:
						return Member1DType.Beam;

					case MemberType.Column:
						return Member1DType.Column;
				}

				throw new NotImplementedException();
			}
		}

		public List<IIdeaElement1D> Elements => new List<IIdeaElement1D>()
		{
			CreateElement(_nodes)
		};

		public string Id => $"member-{UID}";

		public string Name
		{
			get
			{
				if (Properties.Story == 0)
				{
					return Properties.Label.ToString();
				}

				return $"{_objectFactory.GetStory(Properties.Story).lLevel}-{Properties.Label}";
			}
		}

		public IIdeaPersistenceToken Token => null;

		public abstract int UID { get; }

		public abstract MemberType MemberType { get; }

		protected abstract RamMemberProperties Properties { get; }

		private readonly IObjectFactory _objectFactory;
		private readonly ISectionFactory _sectionProvider;
		private readonly INodes _nodes;

		public AbstractRamMember(IObjectFactory objectFactory, ISectionFactory sectionProvider, INodes nodes)
		{
			_objectFactory = objectFactory;
			_sectionProvider = sectionProvider;
			_nodes = nodes;
		}

		public IEnumerable<IIdeaResult> GetResults()
		{
			return null;
		}

		private RamElement1D CreateElement(INodes nodes)
		{
			(INode startNode, INode endNode) = GetNodes(nodes);

			RamLineSegment3D segment = new RamLineSegment3D()
			{
				MemberUID = UID,
				StartNode = _objectFactory.GetNode(startNode),
				EndNode = _objectFactory.GetNode(endNode)
			};

			IRamSection section = _sectionProvider.GetSection(Properties);

			IdeaVector3D offset;
			if (MemberType == MemberType.Beam)
			{
				offset = new IdeaVector3D(0, 0, section.Height / 2);
			}
			else
			{
				offset = new IdeaVector3D(0, 0, 0);
			}

			RamElement1D element = new RamElement1D()
			{
				Segment = segment,
				MemberUID = UID,
				StartCrossSection = section,
				EndCrossSection = section,
				RotationRx = Properties.Rotation,
				EccentricityBegin = offset,
				EccentricityEnd = offset
			};

			return element;
		}

		private INode FindNode(INodes nodes, SCoordinate coordinate)
		{
			return nodes.GetClosestNode(coordinate.dXLoc, coordinate.dYLoc, coordinate.dZLoc);
		}

		private (INode, INode) GetNodes(INodes nodes)
		{
			(SCoordinate start, SCoordinate end) = GetStartEndCoordinates();
			INode startNode = FindNode(nodes, start);
			INode endNode = FindNode(nodes, end);

			return (startNode, endNode);
		}

		protected abstract (SCoordinate, SCoordinate) GetStartEndCoordinates();
	}
}