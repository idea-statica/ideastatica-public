using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using MathNet.Numerics;
using System;
using Vector = MathNet.Spatial.Euclidean.Vector3D;

namespace IdeaStatiCa.BimImporter.Importers
{
    internal class SegmentImporter : AbstractImporter<IIdeaSegment3D>
    {
        private readonly IPluginLogger _logger;

        private const double Precision = 1e-6;

        public SegmentImporter(IPluginLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaSegment3D segment)
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
                    Point = ctx.Import(arcSegment.ArcPoint)
                };
            }
            else
            {
                throw new NotImplementedException("Segment must be instance either IIdeaLineSegment3D or IIdeaArcSegment3D.");
            }

            iomSegment.StartPoint = ctx.Import(segment.StartNode);
            iomSegment.EndPoint = ctx.Import(segment.EndNode);
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
            double x = Normalize(vector.X);
            double y = Normalize(vector.Y);
            double z = Normalize(vector.Z);

            double mag = Math.Sqrt(x * x + y * y + z * z);

            return new Vector3D()
            {
                X = x / mag,
                Y = y / mag,
                Z = z / mag
            };
        }

        private double Normalize(double value)
        {
            double newValue = value.Round((int)-Math.Log10(Precision));
            if (value != newValue)
            {
                _logger.LogInformation($"Value {value} normalized to {newValue}.");
            }

            return newValue;
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