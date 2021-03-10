using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Diagnostics;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class SegmentImporter : AbstractImporter<IIdeaSegment3D>
	{
		private IImporter<IIdeaNode> _nodeImporter;

		public SegmentImporter(IImporter<IIdeaNode> nodeConverter)
		{
			_nodeImporter = nodeConverter;
		}

		protected override ReferenceElement ImportInternal(ImportContext ctx, IIdeaSegment3D segment)
		{
			if (segment.StartNode.IsSimilarTo(segment.EndNode))
			{
				throw new ConstraintException();
			}

			if(!(segment.LocalCoordinateSystem is CoordSystemByVector))
			{
				throw new ConstraintException();
			}

			Segment3D iomSegment;

			if (segment is IIdeaLineSegment3D)
			{
				iomSegment = new LineSegment3D();
			}
			else if (segment is IIdeaArcSegment3D arcSegment)
			{
				if(arcSegment.ArcPoint.IsSimilarTo(arcSegment.StartNode) || arcSegment.ArcPoint.IsSimilarTo(arcSegment.EndNode))
				{
					throw new ConstraintException();
				}

				iomSegment = new ArcSegment3D()
				{
					Point = _nodeImporter.Import(ctx, arcSegment.ArcPoint)
				};
			}
			else
			{
				throw new ConstraintException();
			}

			iomSegment.StartPoint = _nodeImporter.Import(ctx, segment.StartNode);
			iomSegment.EndPoint = _nodeImporter.Import(ctx, segment.EndNode);
			iomSegment.LocalCoordinateSystem = segment.LocalCoordinateSystem; // TODO: normalization

			ctx.Add(iomSegment);

			return new ReferenceElement(iomSegment);
		}
	}
}