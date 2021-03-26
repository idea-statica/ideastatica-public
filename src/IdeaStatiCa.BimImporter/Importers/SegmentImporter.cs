using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Diagnostics;
using MathNet.Numerics;
using System;
using Vector = MathNet.Spatial.Euclidean.Vector3D;

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

		protected override OpenElementId ImportInternal(ImportContext ctx, IIdeaSegment3D segment)
		{
			if (segment.StartNode.IsAlmostEqual(segment.EndNode))
			{
				throw new ConstraintException("StartNode and EndNode must not be the same node.");
			}

			if (!(segment.LocalCoordinateSystem is CoordSystemByVector coordSystem))
			{
				throw new NotImplementedException("LocalCoordinateSystem must be instance of CoordSystemByVector.");
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
				throw new NotImplementedException("Segment must be instance either IIdeaLineSegment3D or IIdeaArcSegment3D.");
			}

			iomSegment.StartPoint = _nodeImporter.Import(ctx, segment.StartNode);
			iomSegment.EndPoint = _nodeImporter.Import(ctx, segment.EndNode);
			iomSegment.LocalCoordinateSystem = ProcessCoordSystem(coordSystem);

			return iomSegment;
		}

		private CoordSystemByVector ProcessCoordSystem(CoordSystemByVector coordSystem)
		{
			CheckVectorSpaceConstraints(Convert(coordSystem.VecX), Convert(coordSystem.VecY), Convert(coordSystem.VecZ));

			return new CoordSystemByVector()
			{
				VecX = NormalizeVector(coordSystem.VecX),
				VecY = NormalizeVector(coordSystem.VecY),
				VecZ = NormalizeVector(coordSystem.VecZ),
			};
		}

		private Vector Convert(Vector3D v)
		{
			return new Vector(v.X, v.Y, v.Z);
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

		private void CheckVectorSpaceConstraints(Vector a, Vector b, Vector c)
		{
			if (!IsUnitVector(a) || !IsUnitVector(b) || !IsUnitVector(c))
			{
				throw new ConstraintException("LCS basis vectors must have unit length.");
			}

			if (!a.IsPerpendicularTo(b, Precision) || !b.IsPerpendicularTo(c, Precision) || !c.IsPerpendicularTo(a, Precision))
			{
				throw new ConstraintException("LCS basis vectors must be perpendicular to each other.");
			}

			if (GetVectorSpaceOrientation(a, b, c) < 0)
			{
				throw new ConstraintException("LCS must be right handed.");
			}
		}

		private bool IsUnitVector(Vector vector)
		{
			return vector.Length.AlmostEqual(1.0, Precision);
		}

		private double GetVectorSpaceOrientation(Vector a, Vector b, Vector c)
		{
			return a.CrossProduct(b).DotProduct(c);
		}
	}
}