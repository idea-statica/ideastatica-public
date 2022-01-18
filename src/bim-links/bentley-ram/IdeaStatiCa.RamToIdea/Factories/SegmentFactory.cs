using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Geometry;
using MathNet.Numerics;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class SegmentFactory : ISegmentFactory
	{
		private const double Tolerance = 1e-6;

		public List<RamLineSegment3D> CreateSegments(Line line)
		{
			List<RamLineSegment3D> segments = new List<RamLineSegment3D>();

			RamNode prevNode = line.Start;
			IdeaRS.OpenModel.Geometry3D.CoordSystem lcs = CreateCoordinateSystem(line);

			if (line.AllowsIntermediateNodes)
			{
				IOrderedEnumerable<(double Position, RamNode RamNode)> nodes = line.IntermediateNodes
					.OrderBy(x => x.Position);

				foreach ((_, RamNode node) in nodes)
				{
					segments.Add(CreateSegment(prevNode, node, lcs));
					prevNode = node;
				}
			}

			segments.Add(CreateSegment(prevNode, line.End, lcs));

			return segments;
		}

		private RamLineSegment3D CreateSegment(RamNode start, RamNode end, IdeaRS.OpenModel.Geometry3D.CoordSystem lcs)
		{
			return new RamLineSegment3D(start, end, lcs);
		}

		private IdeaRS.OpenModel.Geometry3D.CoordSystem CreateCoordinateSystem(Line line)
		{
			UnitVector3D axisX = line.Vector.Normalize();
			UnitVector3D axisZ, axisY;

			if (Math.Abs(axisX.Z) < Tolerance)
			{
				axisZ = UnitVector3D.ZAxis;
				axisY = axisZ.CrossProduct(axisX);
			}
			else if (Math.Abs(axisX.Z).AlmostEqual(1.0, Tolerance))
			{
				axisZ = UnitVector3D.XAxis;
				axisY = UnitVector3D.Create(0.0, axisX.Z > 0 ? -1 : 1, 0);
			}
			else
			{
				UnitVector3D locationVectorXYProj = UnitVector3D.Create(axisX.X, axisX.Y, 0);
				axisY = locationVectorXYProj.CrossProduct(axisX);
				axisZ = axisX.CrossProduct(axisY);
			}

			return new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
			{
				VecX = ConvertVector(axisX),
				VecY = ConvertVector(axisY),
				VecZ = ConvertVector(axisZ),
			};
		}

		private IdeaRS.OpenModel.Geometry3D.Vector3D ConvertVector(UnitVector3D vec)
		{
			return new IdeaRS.OpenModel.Geometry3D.Vector3D()
			{
				X = vec.X,
				Y = vec.Y,
				Z = vec.Z
			};
		}
	}
}