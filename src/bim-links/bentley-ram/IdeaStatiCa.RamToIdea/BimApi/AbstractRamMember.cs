using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Geometry;
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

		public List<IIdeaElement1D> Elements { get; }

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
		private readonly IGeometry _geometry;
		private readonly ISegmentFactory _segmentFactory;

		public AbstractRamMember(IObjectFactory objectFactory, ISectionFactory sectionProvider, IGeometry geometry, ISegmentFactory segmentFactory)
		{
			_objectFactory = objectFactory;
			_sectionProvider = sectionProvider;
			_geometry = geometry;
			_segmentFactory = segmentFactory;

			Elements = CreateElements();
		}

		public IEnumerable<IIdeaResult> GetResults()
		{
			return null;
		}

		private List<IIdeaElement1D> CreateElements()
		{
			Line line = CreateLine();
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

			List<IIdeaElement1D> elements = new List<IIdeaElement1D>();

			foreach (RamLineSegment3D segment in _segmentFactory.CreateSegments(line))
			{
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

				elements.Add(element);
			}

			return elements;
		}

		private Line CreateLine()
		{
			(SCoordinate start, SCoordinate end) = GetStartEndCoordinates();
			return _geometry.CreateLine(start, end);
		}

		protected abstract (SCoordinate, SCoordinate) GetStartEndCoordinates();
	}
}