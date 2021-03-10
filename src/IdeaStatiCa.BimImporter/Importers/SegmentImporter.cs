using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Diagnostics;
using MathNet.Numerics;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class SegmentImporter : AbstractImporter<IIdeaSegment3D>
	{
		private readonly static IIdeaLogger _logger = IdeaDiagnostics.GetLogger("ideastatica.bimimporter.segmentimporter");

		private const double Precision = 1e-6;

		private readonly IImporter<IIdeaNode> _nodeImporter;

		public SegmentImporter(IImporter<IIdeaNode> nodeConverter)
		{
			_nodeImporter = nodeConverter;
		}

		protected override ReferenceElement ImportInternal(ImportContext ctx, IIdeaSegment3D segment)
		{
			if (segment.StartNode.IsAlmostEqual(segment.EndNode))
			{
				throw new ConstraintException("StartNode and EndNode must not be the same node.");
			}

			if (!(segment.LocalCoordinateSystem is CoordSystemByVector coordSystem))
			{
				throw new ConstraintException("LocalCoordinateSystem must be instance of CoordSystemByVector.");
			}

			Segment3D iomSegment;

			if (segment is IIdeaLineSegment3D)
			{
				iomSegment = new LineSegment3D();
			}
			else if (segment is IIdeaArcSegment3D arcSegment)
			{
				if (arcSegment.ArcPoint.IsAlmostEqual(arcSegment.StartNode) || arcSegment.ArcPoint.IsAlmostEqual(arcSegment.EndNode))
				{
					throw new ConstraintException("ArcPoint must not be the same as StartNode or EndNode.");
				}

				iomSegment = new ArcSegment3D()
				{
					Point = _nodeImporter.Import(ctx, arcSegment.ArcPoint)
				};
			}
			else
			{
				throw new ConstraintException("Segment must be instance either IIdeaLineSegment3D or IIdeaArcSegment3D.");
			}

			iomSegment.StartPoint = _nodeImporter.Import(ctx, segment.StartNode);
			iomSegment.EndPoint = _nodeImporter.Import(ctx, segment.EndNode);
			iomSegment.LocalCoordinateSystem = ProcessCoordSystem(coordSystem);

			ctx.Add(iomSegment);

			return new ReferenceElement(iomSegment);
		}

		private CoordSystemByVector ProcessCoordSystem(CoordSystemByVector coordSystem)
		{
			if (!IsUnitVector(coordSystem.VecX) || !IsUnitVector(coordSystem.VecY) || !IsUnitVector(coordSystem.VecZ))
			{
				throw new ConstraintException("LCS basis vectors must have unit length.");
			}

			return new CoordSystemByVector()
			{
				VecX = NormalizeVector(coordSystem.VecX),
				VecY = NormalizeVector(coordSystem.VecY),
				VecZ = NormalizeVector(coordSystem.VecZ),
			};
		}

		private Vector3D NormalizeVector(Vector3D vector)
		{
			return new Vector3D()
			{
				X = Normalize(vector.X),
				Y = Normalize(vector.Y),
				Z = Normalize(vector.Z),
			};
		}

		private bool IsUnitVector(Vector3D vector)
		{
			double lengthSquared = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
			return lengthSquared.AlmostEqual(1.0, Precision);
		}

		private double Normalize(double value)
		{
			if (value.AlmostEqual(1.0, Precision))
			{
				_logger.LogInformation($"Normalizing {value} to 1.0.");
				return 1.0;
			}
			else if (value.AlmostEqual(0.0, Precision))
			{
				_logger.LogInformation($"Normalizing {value} to 0.0.");
				return 0.0;
			}
			else if (value.AlmostEqual(-1.0, Precision))
			{
				_logger.LogInformation($"Normalizing {value} to -1.0.");
				return -1.0;
			}

			return value;
		}
	}
}