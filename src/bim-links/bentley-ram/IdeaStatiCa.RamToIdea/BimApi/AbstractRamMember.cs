using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.RamToIdea.Factories;
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

		public string Name => Label.ToString();

		public IIdeaPersistenceToken Token => new PersistenceToken(UID, MemberType);

		public abstract int UID { get; }

		public abstract MemberType MemberType { get; }
		protected virtual double Rotation { get; }

		protected abstract int Label { get; }

		protected abstract EMATERIALTYPES MaterialType { get; }

		protected abstract int MaterialUID { get; }

		protected abstract string CrossSectionName { get; }

		protected abstract int CrossSectionUID { get; }

		private readonly IObjectFactory _objectFactory;
		private readonly INodes _nodes;

		public AbstractRamMember(IObjectFactory objectFactory, INodes nodes)
		{
			_objectFactory = objectFactory;
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

			IIdeaMaterial material = _objectFactory.GetMaterial(MaterialType, MaterialUID);
			RamCrossSectionByName css = new RamCrossSectionByName()
			{
				Id = $"css-{CrossSectionUID}",
				Name = CrossSectionName,
				Material = material
			};

			RamElement1D element = new RamElement1D()
			{
				Segment = segment,
				MemberUID = UID,
				StartCrossSection = css,
				EndCrossSection = css,
				RotationRx = Rotation
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