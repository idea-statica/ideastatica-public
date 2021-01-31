using System;
using CI.Mathematics;

namespace CI.Geometry3D
{
	internal class ParabolaProperty
	{
		internal ParabolaProperty(IArcSegment3D parabolicArc)
		{
			Matrix = new Matrix44();
			IsValid = CalculateParabolaProperty(parabolicArc);
			if (!IsValid)
			{
				var seg = new ArcSegment3D(parabolicArc.StartPoint, parabolicArc.IntermedPoint, parabolicArc.EndPoint);
				ParabolaLength = GeomOperation.GetLength(seg);
			}
		}

		internal double ParabolaLength { get; set; }

		internal Matrix44 Matrix { get; set; }

		internal double ParabolaAxisAngle { get; set; }

		internal double ParabolicArcX1 { get; set; }

		internal double ParabolicArcX2 { get; set; }

		internal bool IsValid { get; set; }

		/// <summary>
		/// Calculate the parabola for given relative position.
		/// </summary>
		/// <param name="relativePosition">relative position for a given parabola</param>
		/// <returns>length of parabola for given relative position</returns>
		internal double GetLengthToCurve(double relativePosition)
		{
			if (ParabolaAxisAngle.IsZero() || Math.Abs(ParabolaAxisAngle).IsGreater(1e8))
			{
				return relativePosition;
			}

			double p = 2 * ParabolaAxisAngle * Math.Abs(relativePosition);
			double b = Math.Sqrt(1 + (p * p));
			double relativeParabolaLength = ((p * b) + Math.Log(p + b)) / (4 * ParabolaAxisAngle);
			//if (relativePosition.IsLesser(0.0))
			if (relativePosition < 0.0)
			{
				relativeParabolaLength = -relativeParabolaLength;
			}

			return relativeParabolaLength;
		}

		/// <summary>
		/// Calculate Point on parabola.
		/// </summary>
		/// <param name="segmentParabola">Segment Parabola</param>
		/// <param name="relativePosition">Relative Position</param>
		/// <param name="point">Point On Parabola</param>
		/// <returns>true if given input are valid</returns>
		internal bool GetPointOnParabola(IArcSegment3D segmentParabola, double relativePosition, ref IPoint3D point)
		{
			if (segmentParabola == null)
			{
				throw new ArgumentNullException();
			}

			if (!IsValid)
			{
				IPoint3D a = segmentParabola.StartPoint;
				IPoint3D b = segmentParabola.IntermedPoint;

				point = GeomOperation.Add(GeomOperation.Multiply(b, relativePosition), GeomOperation.Multiply(a, 1 - relativePosition));
				return true;
			}

			double x = 0;

			// CIH - nejak jsem ty Abs nepochopil
			// if (Math.Abs(relativePosition).IsLesser(0.0))
			// {
			//   x = parabolaProperty.ParabolicArcX1;
			// }
			// else if (Math.Abs(relativePosition - 1.0).IsLesser(0.0))
			// {
			//   x = parabolaProperty.ParabolicArcX2;
			// }
			// else if (Math.Abs(parabolaProperty.ParabolaAxisAngle).IsLesser(0.0))
			if (relativePosition.IsEqual(0.0))
			{
				x = ParabolicArcX1;
			}
			else if (relativePosition.IsEqual(1.0))
			{
				x = ParabolicArcX2;
			}
			//else if (ParabolaAxisAngle.IsLesser(0.0))
			//{
			//  x = relativePosition * (ParabolicArcX2 - ParabolicArcX1) + ParabolicArcX1;
			//}
			else
			{
				x = 0.0;
				int it = 100;
				double pom = relativePosition * ParabolaLength + GetLengthToCurve(ParabolicArcX1);
				double caa = 4 * ParabolaAxisAngle * ParabolaAxisAngle;
				double len = pom;

				do
				{
					x += len / Math.Sqrt(1 + caa * x * x);
					len = pom - GetLengthToCurve(x);
				}
				while ((!len.IsZero(MathConstants.ZeroGeneral)) && (--it > 0));
				//while ((!Math.Abs(len).IsLesser(0.0)) && (--it > 0));

				if (Math.Abs(len) > 1e-8)
				{
					return false;
				}
			}

			Vector3D normline = Matrix.AxisX;
			Vector3D ortho = Matrix.AxisY;
			point = GeomOperation.Add(segmentParabola.IntermedPoint, normline * x + ParabolaAxisAngle * ortho * x * x);
			return true;
		}

		/// <summary>
		/// Calculate the property of parabola.
		/// </summary>
		/// <param name="parabolicArc">Segment Parabola</param>
		/// <returns>true if given parabola segment is valid to calculate Length</returns>
		private bool CalculateParabolaProperty(IArcSegment3D parabolicArc)
		{
			Vector3D cb = GeomOperation.Subtract(parabolicArc.EndPoint, parabolicArc.IntermedPoint);
			Vector3D ncb = cb.Normalize;
			Vector3D ca = GeomOperation.Subtract(parabolicArc.StartPoint, parabolicArc.IntermedPoint);
			Vector3D nca = ca.Normalize;
			double d = ~(ncb * nca);
			//if (Math.Abs(d).IsLesser(0.0, MathConstants.ZeroGeneral))
			if (Math.Abs(d).IsZero(1e-5))
			{
				Vector3D vv = GeomOperation.Subtract(parabolicArc.EndPoint, parabolicArc.StartPoint);
				Vector3D normalizedLine = vv.Normalize;

				double parabolaX1 = normalizedLine | ca;
				double parabolaX2 = normalizedLine | cb;

				if (parabolaX2 < parabolaX1)
				{
					normalizedLine = -normalizedLine;
					parabolaX1 = -parabolaX1;
					parabolaX2 = -parabolaX2;
				}

				ParabolaAxisAngle = 0.0;
				ParabolicArcX1 = parabolaX1;
				ParabolicArcX2 = parabolaX2;

				ParabolaLength = vv.Magnitude;

				//SwapParabolicSegment(ref normalizedLine, ref parabolaX1, ref parabolaX2);
				//ParabolaLength = ~vv;

				//ParabolicArcX1 = normalizedLine | ca;
				//ParabolicArcX2 = normalizedLine | cb;

				//double parX1 = ParabolicArcX1;
				//double parX2 = ParabolicArcX2;
				//SwapParabolicSegment(ref normalizedLine, ref parX1, ref parX2);
				//ParabolicArcX1 = parX1;
				//ParabolicArcX2 = parX2;

				//Vector3D localY = new Vector3D(0, 0, 0);
				//if ((normalizedLine.DirectionZ - 1.0).IsZero())
				//{
				//  localY.DirectionY = 1.0;
				//}
				//else if (normalizedLine.DirectionZ.IsZero())
				//{
				//  localY.DirectionZ = 1.0;
				//}
				//else
				//{
				//  localY.DirectionX = -normalizedLine.DirectionX;
				//  localY.DirectionY = -normalizedLine.DirectionY;
				//  localY.DirectionZ = ((normalizedLine.DirectionX * normalizedLine.DirectionX) + (normalizedLine.DirectionY * normalizedLine.DirectionY)) / normalizedLine.DirectionZ;
				//  localY = localY.Normalize;
				//}

				//Vector3D vectOrtho = GeomOperation.Rotate(localY, normalizedLine, 0.0);

				//ParabolaAxisAngle = (vectOrtho | ca) / (ParabolicArcX1 * ParabolicArcX1);

				//Matrix.AxisX = normalizedLine;
				//Matrix.AxisY = vectOrtho;
				//Matrix.AxisZ = (normalizedLine * vectOrtho).Normalize;

				//ParabolaLength = GetLengthToCurve(ParabolicArcX2) - GetLengthToCurve(ParabolicArcX1);
				//ParabolaLength = ~vv;

				return false;
			}

			double parabolaAngle = 0.0;
			Vector3D vectorOrtho = new Vector3D();
			Vector3D normalizedVector = new Vector3D();

			bool repeat = false;
			do
			{
				if (!CalculateParabolaPlane(ca, cb, ref parabolaAngle, ref vectorOrtho, ref normalizedVector, repeat))
				{
					return false;
				}

				Vector3D normalLine = vectorOrtho * normalizedVector;

				double parabolaX1 = normalLine | ca;
				double parabolaX2 = normalLine | cb;

				if (parabolaX2 < parabolaX1)
				{
					normalLine = -normalLine;
					parabolaX1 = -parabolaX1;
					parabolaX2 = -parabolaX2;
				}

				ParabolicArcX1 = parabolaX1;
				ParabolicArcX2 = parabolaX2;

				//ParabolicArcX1 = normalLine | ca;
				//ParabolicArcX2 = normalLine | cb;

				//double parX1 = ParabolicArcX1;
				//double parX2 = ParabolicArcX2;
				//SwapParabolicSegment(ref normalLine, ref parX1, ref parX2);
				//ParabolicArcX1 = parX1;
				//ParabolicArcX2 = parX2;

				ParabolaAxisAngle = (vectorOrtho | ca) / (ParabolicArcX1 * ParabolicArcX1);

				Matrix.AxisX = normalLine;
				Matrix.AxisY = vectorOrtho;
				Matrix.AxisZ = normalizedVector;

				ParabolaLength = GetLengthToCurve(ParabolicArcX2) - GetLengthToCurve(ParabolicArcX1);
				double d1 = Math.Sqrt(((parabolicArc.StartPoint.X - parabolicArc.IntermedPoint.X) * (parabolicArc.StartPoint.X - parabolicArc.IntermedPoint.X)) + ((parabolicArc.StartPoint.Y - parabolicArc.IntermedPoint.Y) * (parabolicArc.StartPoint.Y - parabolicArc.IntermedPoint.Y)) + ((parabolicArc.StartPoint.Z - parabolicArc.IntermedPoint.Z) * (parabolicArc.StartPoint.Z - parabolicArc.IntermedPoint.Z)));
				double d2 = Math.Sqrt(((parabolicArc.EndPoint.X - parabolicArc.IntermedPoint.X) * (parabolicArc.EndPoint.X - parabolicArc.IntermedPoint.X)) + ((parabolicArc.EndPoint.Y - parabolicArc.IntermedPoint.Y) * (parabolicArc.EndPoint.Y - parabolicArc.IntermedPoint.Y)) + ((parabolicArc.EndPoint.Z - parabolicArc.IntermedPoint.Z) * (parabolicArc.EndPoint.Z - parabolicArc.IntermedPoint.Z)));
				if (!repeat)
				{
					if (ParabolaLength.IsLesser(d1 + d2))
					{
						repeat = true;
					}
				}
				else
				{
					repeat = false;
				}
			}
			while (repeat);

			return true;
		}

		/// <summary>
		/// Swap the parabolic segment.
		/// </summary>
		/// <param name="normalLine">Base Vector for Parabola</param>
		/// <param name="parabolaX1">Perpendicular distance between start and ortho line</param>
		/// <param name="parabolaX2">Perpendicular distance between end and ortho line</param>
		private void SwapParabolicSegment(ref Vector3D normalLine, ref double parabolaX1, ref double parabolaX2)
		{
			if (parabolaX2 < parabolaX1)
			{
				normalLine = -normalLine;
				parabolaX1 = -parabolaX1;
				parabolaX2 = -parabolaX2;
			}
		}

		/// <summary>
		/// Calculate the plane of parabola.
		/// </summary>
		/// <param name="ca">Direction Vector between start and intermediate</param>
		/// <param name="cb">Direction Vector between end and intermediate</param>
		/// <param name="angleOfParabola">Axis angle of parabola</param>
		/// <param name="vectorOrtho">Perpendicular Vector</param>
		/// <param name="normalizedVector">Parabola Normal</param>
		/// <param name="considerAngle">Calculate Parabola plane by angle</param>
		/// <returns>true if parabola is in plane</returns>
		private bool CalculateParabolaPlane(Vector3D ca, Vector3D cb, ref double angleOfParabola, ref Vector3D vectorOrtho, ref Vector3D normalizedVector, bool considerAngle)
		{
			ParabolaComputation computedParabola = new ParabolaComputation(ca, cb);
			int result = ComputeParabola(ref computedParabola, considerAngle);
			if (result == -1)
			{
				return false;
			}

			angleOfParabola = computedParabola.ResultCollection[result];
			vectorOrtho = computedParabola.GetY(angleOfParabola);
			normalizedVector = computedParabola.NormalizedVector;
			//normalizedVector = vectorOrtho * computedParabola.NormalizedVector;

			return true;
		}

		/// <summary>
		/// Compute parabola operations.
		/// </summary>
		/// <param name="computedParabola">Parabola to be computed</param>
		/// <param name="considerAngle">Calculate Parabola plane by angle</param>
		/// <returns>true if it is valid parabola</returns>
		private int ComputeParabola(ref ParabolaComputation computedParabola, bool considerAngle)
		{
			if (computedParabola.IterateParabola() < 1)
			{
				return -1;
			}

			int min = -1;
			double minDiff = 1e10;
			bool sameSide = false;
			for (int i = 0; i < computedParabola.MaximumResult; i++)
			{
				if (!considerAngle)
				{
					bool same = false;
					double diff = computedParabola.GetAngleStateCount(i, ref same);

					if (diff.IsEqual(-1))
					{
						continue;
					}

					if (sameSide && !same)
					{
						continue;
					}

					//if (diff.IsLesser(minDiff) || (!sameSide && same))
					if (diff < minDiff || (!sameSide && same))
					{
						min = i;
						minDiff = diff;
						sameSide = same;
					}
				}
				else
				{
					double diff = computedParabola.GetAngleState(i, ref sameSide, ParabolaAxisAngle);
					if (diff.IsEqual(-1)) //TODO porovnani na presnost ?
					{
						continue;
					}

					//if (!computedParabola.GetAngleState(i, ref sameSide, ref diff))
					//{
					//  continue;
					//}

					//if (diff.IsLesser(minDiff))
					if (diff < minDiff)
					{
						min = i;
						minDiff = diff;
					}
				}
			}

			return min;
		}
	}
}
