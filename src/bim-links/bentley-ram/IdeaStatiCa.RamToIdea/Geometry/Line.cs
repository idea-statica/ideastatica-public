using IdeaStatiCa.RamToIdea.BimApi;
using MathNet.Numerics;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal class Line
	{
		private const double Tolerance = 1e-6;

		private readonly int uid;

		public RamNode Start { get; }

		public RamNode End { get; }

		public Vector3D Vector { get; }

		public bool AllowsIntermediateNodes => !(_intermediateNodes is null);

		public IReadOnlyCollection<(double Position, RamNode RamNode)> IntermediateNodes => _intermediateNodes;

		public IdeaRS.OpenModel.Geometry3D.CoordSystemByVector LCS { get; }

		private readonly List<(double, RamNode)> _intermediateNodes = new List<(double, RamNode)>();

		public Line(int uid, RamNode start, RamNode end, bool allowsIntermediateNode)
		{
			this.uid = uid;
			Start = start;
			End = end;

			Vector = end.Position - start.Position;

			if (!allowsIntermediateNode)
			{
				_intermediateNodes = null;
			}

			LCS = GetCoordinateSystem(Vector);
		}

		public void AddIntermediateNode(double position, RamNode node)
		{
			if (_intermediateNodes is null)
			{
				throw new InvalidOperationException();
			}

			_intermediateNodes.Add((position, node));
		}

		public bool ContainsNode(RamNode ramNode)
		{
			string id = ramNode.Id;

			if (id == Start.Id || id == End.Id)
			{
				return true;
			}

			return _intermediateNodes.Any(x => x.Item2.Id == id);
		}

		private static IdeaRS.OpenModel.Geometry3D.CoordSystemByVector GetCoordinateSystem(Vector3D directionVector)
		{
			UnitVector3D axisX = directionVector.Normalize();
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
				axisY = axisX.CrossProduct(locationVectorXYProj);
				axisZ = axisX.CrossProduct(axisY);
			}

			return new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
			{
				VecX = ConvertVector(axisX),
				VecY = ConvertVector(axisY),
				VecZ = ConvertVector(axisZ)
			};
		}

		private static IdeaRS.OpenModel.Geometry3D.Vector3D ConvertVector(UnitVector3D vec)
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