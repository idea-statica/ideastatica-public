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

			var nodes = line.IntermediateNodes
				.OrderBy(x => x.Position);

			foreach ((_, RamNode node) in nodes)
			{
				segments.Add(CreateSegment(prevNode, node, lcs));
				prevNode = node;
			}

			segments.Add(CreateSegment(prevNode, line.End, lcs));

			return segments;
		}

		private RamLineSegment3D CreateSegment(RamNode start, RamNode end, IdeaRS.OpenModel.Geometry3D.CoordSystem lcs)
		{
			return new RamLineSegment3D(
				start,
				end,
				  lcs);
		}

		private IdeaRS.OpenModel.Geometry3D.CoordSystem CreateCoordinateSystem(Line line)
		{
			UnitVector3D axisX = line.Vector.Normalize();
			UnitVector3D axisY = GetNormalVector(axisX);
			UnitVector3D axisZ = axisX.CrossProduct(axisY);

			return new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
			{
				VecX = ConvertVector(axisX),
				VecY = ConvertVector(axisY),
				VecZ = ConvertVector(axisZ),
			};
		}

		private UnitVector3D GetNormalVector(UnitVector3D directionVector)
		{
			if (Math.Abs(directionVector.Z) < Tolerance)
			{
				return UnitVector3D.Create(0, 0, 1.0);
			}

			if (Math.Abs(directionVector.Z).AlmostEqual(1.0, Tolerance))
			{
				return UnitVector3D.Create(0, 0, directionVector.Z > 0 ? 1.0 : -1.0);
			}

			if (directionVector.Z < 0)
			{
				directionVector = directionVector.Negate();
			}

			UnitVector3D locationVectorXYProj = UnitVector3D.Create(directionVector.X, directionVector.Y, 0);
			UnitVector3D axisY = locationVectorXYProj.CrossProduct(directionVector);

			return axisY.CrossProduct(directionVector);
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