using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Extensions;
using IdeaStatiCa.Plugin;
using MathNet.Numerics;
using System;
using Vector = MathNet.Spatial.Euclidean.Vector3D;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class SegmentImporter : AbstractImporter<IIdeaSegment3D>
	{
		private readonly IPluginLogger _logger;		

		public SegmentImporter(IPluginLogger logger) : base(logger)
		{
			_logger = logger;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaSegment3D segment)
		{
			double geometryPrecision = ctx.Configuration.GeometryPrecision;
			double lcsPrecision = ctx.Configuration.LCSPrecision;
			double lcsPrecisionForNormalization = ctx.Configuration.LCSPrecisionForNormalization;

			if (segment.StartNode.IsAlmostEqual(segment.EndNode, geometryPrecision))
			{
				throw new ConstraintException("StartNode and EndNode must not be the same node.");
			}

			if (segment.LocalCoordinateSystem != null && !(segment.LocalCoordinateSystem is CoordSystemByVector))
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
				if (arcSegment.ArcPoint.IsAlmostEqual(arcSegment.StartNode, geometryPrecision)
					|| arcSegment.ArcPoint.IsAlmostEqual(arcSegment.EndNode, geometryPrecision))
				{
					throw new ConstraintException("ArcPoint must not be the same as StartNode or EndNode.");
				}

				iomSegment = new ArcSegment3D()
				{
					Point = ctx.Import(arcSegment.ArcPoint)
				};
			}
			else
			{
				throw new NotImplementedException("Segment must be instance either IIdeaLineSegment3D or IIdeaArcSegment3D.");
			}

			iomSegment.StartPoint = ctx.Import(segment.StartNode);
			iomSegment.EndPoint = ctx.Import(segment.EndNode);
			if (segment.LocalCoordinateSystem != null)
			{
				iomSegment.LocalCoordinateSystem = ProcessCoordSystem(segment.LocalCoordinateSystem as CoordSystemByVector, lcsPrecision, 
																		lcsPrecisionForNormalization);
			}

			return iomSegment;
		}

		private CoordSystemByVector ProcessCoordSystem(CoordSystemByVector coordSystem, double lcsPrecision, 
														double lcsPrecisionForNormalization)
		{
			CheckVectorSpaceConstraints(
				Convert(coordSystem.VecX),
				Convert(coordSystem.VecY),
				Convert(coordSystem.VecZ),
				lcsPrecision);

			return new CoordSystemByVector()
			{
				VecX = NormalizeVector(coordSystem.VecX, lcsPrecisionForNormalization),
				VecY = NormalizeVector(coordSystem.VecY, lcsPrecisionForNormalization),
				VecZ = NormalizeVector(coordSystem.VecZ, lcsPrecisionForNormalization),
			};
		}

		private Vector Convert(Vector3D v)
		{
			return new Vector(v.X, v.Y, v.Z);
		}

		private Vector3D NormalizeVector(Vector3D vector, double lcsPrecisionForNormalization)
		{
			double x = Normalize(vector.X, lcsPrecisionForNormalization);
			double y = Normalize(vector.Y, lcsPrecisionForNormalization);
			double z = Normalize(vector.Z, lcsPrecisionForNormalization);

			double mag = Math.Sqrt(x * x + y * y + z * z);

			return new Vector3D()
			{
				X = x / mag,
				Y = y / mag,
				Z = z / mag
			};
		}

		private double Normalize(double value, double lcsPrecisionForNormalization)
		{
			double newValue = value.Round(lcsPrecisionForNormalization.LeadingDecimalZeros());
			if (value != newValue)
			{
				_logger.LogTrace($"Value {value} normalized to {newValue}.");
			}

			return newValue;
		}

		private void CheckVectorSpaceConstraints(Vector a, Vector b, Vector c, double lcsPrecision)
		{
			if (!IsUnitVector(a, lcsPrecision) || !IsUnitVector(b, lcsPrecision) || !IsUnitVector(c, lcsPrecision))
			{
				throw new ConstraintException("LCS basis vectors must have unit length.");
			}

			if (!a.IsPerpendicularTo(b, lcsPrecision)
				|| !b.IsPerpendicularTo(c, lcsPrecision)
				|| !c.IsPerpendicularTo(a, lcsPrecision))
			{
				throw new ConstraintException("LCS basis vectors must be perpendicular to each other.");
			}

			if (GetVectorSpaceOrientation(a, b, c) < 0)
			{
				throw new ConstraintException("LCS must be right handed.");
			}
		}

		private bool IsUnitVector(Vector vector, double lcsPrecision)
		{
			return vector.Length.AlmostEqual(1.0, lcsPrecision);
		}

		private double GetVectorSpaceOrientation(Vector a, Vector b, Vector c)
		{
			return a.CrossProduct(b).DotProduct(c);
		}
	}
}