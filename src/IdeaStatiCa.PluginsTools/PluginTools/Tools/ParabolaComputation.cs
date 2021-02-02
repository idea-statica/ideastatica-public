using System;
using System.Collections.Generic;
using CI.Mathematics;

namespace CI.Geometry3D
{
	/// <summary>
	/// Parabola Computation.
	/// </summary>
	internal class ParabolaComputation
	{
		/// <summary>
		/// Maximum result count.
		/// </summary>
		private const int ResultCount = 6;//7;

		/// <summary>
		/// Normal vector.
		/// </summary>
		private Vector3D normalizedVector;

		/// <summary>
		/// Collection of results.
		/// </summary>
		private IList<double> resultCollection;

		/// <summary>
		/// Maximum results.
		/// </summary>
		private int maximumResult;

		/// <summary>
		/// Vector Local Y.
		/// </summary>
		private Vector3D localY;

		/// <summary>
		/// Vector local X.
		/// </summary>
		private Vector3D localX;

		/// <summary>
		/// Direction Vector of start and intermediate.
		/// </summary>
		private Vector3D vectorCA;

		/// <summary>
		/// Direction vector of end and intermediate.
		/// </summary>
		private Vector3D vectorCB;

		/// <summary>
		/// Normalized Direction Vector of start and intermediate.
		/// </summary>
		private Vector3D vectorNormalCA;

		/// <summary>
		/// Normalized Direction Vector of end and intermediate.
		/// </summary>
		private Vector3D vectorNormalCB;

		/// <summary>
		/// Constructor Parabola.
		/// </summary>
		/// <param name="ca">Vector between start and intermediate</param>
		/// <param name="cb">Vector between end and intermediate</param>
		internal ParabolaComputation(Vector3D ca, Vector3D cb)
		{
			resultCollection = new List<double>();
			for (int i = 0; i < ResultCount; i++)
			{
				resultCollection.Add(0);
			}

			maximumResult = 0;
			normalizedVector = (cb * ca).Normalize;
			if (normalizedVector.DirectionZ.IsLesser(0.0, MathConstants.ZeroGeneral))
			{
				normalizedVector = -normalizedVector;
			}

			if ((normalizedVector.DirectionZ - 1.0).IsZero())
			{
				localY.DirectionY = 1.0;
			}
			else if (normalizedVector.DirectionZ.IsZero())
			{
				localY.DirectionZ = 1.0;
			}
			else
			{
				localY.DirectionX = -normalizedVector.DirectionX;
				localY.DirectionY = -normalizedVector.DirectionY;
				localY.DirectionZ = ((normalizedVector.DirectionX * normalizedVector.DirectionX) + (normalizedVector.DirectionY * normalizedVector.DirectionY)) / normalizedVector.DirectionZ;
				localY = localY.Normalize;
			}

			localX = localY * normalizedVector;
			vectorCA = ca;
			vectorCB = cb;
			vectorNormalCA = normalizedVector * vectorCA;
			vectorNormalCB = normalizedVector * vectorCB;
		}

		/// <summary>
		/// Normal vector.
		/// </summary>
		internal Vector3D NormalizedVector
		{
			get
			{
				return normalizedVector;
			}
		}

		/// <summary>
		/// collection of result.
		/// </summary>
		internal IList<double> ResultCollection
		{
			get
			{
				return resultCollection;
			}
		}

		/// <summary>
		/// Maximum results.
		/// </summary>
		internal int MaximumResult
		{
			get
			{
				return maximumResult;
			}
		}

		///// <summary>
		///// Iterate parabola.
		///// </summary>
		///// <returns>Parabola segment count</returns>
		//internal int IterateParabola()
		//{
		//  double distanceOffset = 0.0;
		//  double fx, minFx;
		//  do
		//  {
		//    int cnt = 0;
		//    int bad = 0;
		//    double fx1;
		//    double alpha = distanceOffset;
		//    double min = 0.0;

		//    double diff;

		//    minFx = fx = ComputeParabolaAngle(alpha);
		//    while ((cnt++ < 25) && (bad < 5) && !fx.IsZero(MathConstants.ZeroMaximum))
		//    {
		//      fx1 = ComputeParabolaAngle(alpha + MathConstants.ZeroGeneral);

		//      if ((fx - fx1).IsZero(MathConstants.ZeroMaximum))
		//      {
		//        alpha += .5;
		//      }
		//      else
		//      {
		//        diff = (MathConstants.ZeroGeneral * fx) / (fx - fx1);
		//        if (Math.Abs(diff).IsGreater(1.0))
		//        {
		//          diff /= 2.0;
		//        }

		//        alpha += diff;
		//      }

		//      if (!(alpha >= -Math.PI && alpha < Math.PI))
		//      {
		//        if (alpha < -Math.PI)
		//        {
		//          if (alpha > -3.0 * Math.PI)
		//          {
		//            alpha += Math.PI * 2;
		//          }
		//          else
		//          {
		//            alpha = 0.1;
		//          }
		//        }
		//        else if (alpha >= Math.PI)
		//        {
		//          if (alpha < 3.0 * Math.PI)
		//          {
		//            alpha -= Math.PI * 2;
		//          }
		//          else
		//          {
		//            alpha = 0.1;
		//          }
		//        }
		//        else
		//        {
		//          alpha = 0.1;
		//        }
		//      }

		//      fx = ComputeParabolaAngle(alpha);
		//      if (Math.Abs(fx).IsGreater(Math.Abs(minFx), MathConstants.ZeroWeak))
		//      {
		//        bad++;
		//      }
		//      else
		//      {
		//        bad = 0;
		//        minFx = fx;
		//        min = alpha;
		//      }
		//    }

		//    if (!minFx.IsZero(MathConstants.ZeroGeneral))
		//    {
		//      if (distanceOffset.IsLesser(Math.PI * 2))
		//      {
		//        distanceOffset += 1.2;
		//      }
		//      else
		//      {
		//        break;
		//      }
		//    }
		//    else
		//    {
		//      if (!((maximumResult >= 0) && (maximumResult < ResultCount)))
		//      {
		//        throw new NotImplementedException();
		//      }

		//      maximumResult++;
		//      resultCollection.Add(min);
		//    }
		//  }
		//  while (maximumResult < ResultCount);
		//  return maximumResult;
		//}

		/// <summary>
		/// Iterate parabola.
		/// </summary>
		/// <param name="startVal">start value</param>
		/// <returns>Parabola segment count</returns>
		internal int IterateParabola(double startVal = 0.0)
		{
			int cntMain = 0;
			int state = startVal == 0.0 ? 1 : 0;
			do
			{
				int cnt = 0;
				int bad = 0;

				double alpha = startVal;
				double min = startVal;

				double fx = ComputeParabolaAngle(alpha);
				double minFx = fx;

				while ((cnt++ < 25) && (bad < 3) && !minFx.IsZero(1e-16))
				{
					double dFx1 = ComputeParabolaAngle(alpha + 1e-9);

					//double diff = ( PAR_ZERO * dFx ) / (dFx - dFx1);

					double r = 1 - dFx1 / fx;
					r /= 1e-9;

					double diff = r.IsZero(1e-12) ? 0.5 : (1.0 / r);
					if (Math.Abs(diff) > 1.0)
					{
						diff /= 2.0;
					}

					alpha += diff;

					alpha = NormalizeParabolaAngle(alpha);

					fx = ComputeParabolaAngle(alpha);
					if (Math.Abs(fx) >= Math.Abs(minFx))
					{
						bad++;
					}
					else
					{
						bad = 0;
						minFx = fx;
						min = alpha;
					}
				}

				if (!minFx.IsZero(MathConstants.ZeroGeneral))
				{
					if (state == 0)
					{
						state = 1;
						startVal = -Math.PI;
					}
					else
					{
						if (startVal < Math.PI * 2.0)
							startVal += 1.0;
						else
							break;
					}
				}
				else
				{
					AddResult(min);
					min += (min < 0.0) ? Math.PI : -Math.PI;
					AddResult(min);
				}
			}
			while ((cntMain++ < 50) && (maximumResult < ResultCount));

			return maximumResult;
		}

		void AddResult(double d)
		{
			if (maximumResult >= ResultCount)
			{
				return;
			}

			for (int i = 0; i < maximumResult; i++)
			{
				if ((d - resultCollection[i]).IsZero(MathConstants.ZeroGeneral))
					return;
			}

			//ASSERT((m_iMaxRes >= 0) && (m_iMaxRes < RES_CNT));
			//result[m_iMaxRes++] = d;

			resultCollection[maximumResult++] = d;
		}

		private double NormalizeParabolaAngle(double angle)
		{
			if (Math.Abs(angle) > 20.0)
			{
				return 0.0;
			}

			while (angle > Math.PI)
			{
				angle -= Math.PI * 2.0;
			}

			while (angle < -Math.PI)
			{
				angle += Math.PI * 2.0;
			}

			return angle;
		}

		/// <summary>
		/// Calculate perpendicular vector of parabola.
		/// </summary>
		/// <param name="axisAngle">Axis Angle</param>
		/// <returns>perpendicular vector of parabola</returns>
		internal Vector3D GetY(double axisAngle)
		{
			return GeomOperation.Rotate(localY, normalizedVector, axisAngle);
		}

		/// <summary>
		/// Calculate Angle State Count.
		/// </summary>
		/// <param name="segmentCount">Segment Count</param>
		/// <param name="sameSide">State of Parabola</param>
		/// <returns>Angle State Count</returns>
		internal double GetAngleStateCount(int segmentCount, ref bool sameSide)
		{
			IPoint3D start = new Point3D();
			IPoint3D end = new Point3D();
			return GetCoordinates(segmentCount, ref start, ref end, ref sameSide) ? Math.Abs(start.X - end.X) : -1;
		}

		private double GetAngleDiff(int i, double angle)
		{
			double diff = Math.Abs(angle - ResultCollection[i]);
			while (diff > Math.PI)
			{
				diff -= Math.PI * 2;
			}

			return Math.Abs(diff);
		}

		/// <summary>
		/// Calculate State Angle.
		/// </summary>
		/// <param name="segmentCount">Segment Count</param>
		/// <param name="sameSide">State of Parabola</param>
		/// <param name="angle">State Angle</param>
		/// <returns>Check State Angle is valid</returns>
		internal double GetAngleState(int segmentCount, ref bool sameSide, double angle)
		{
			//stateAngle = Math.Abs(resultCollection[segmentCount]);
			//if (stateAngle.IsGreater(Math.PI, MathConstants.ZeroWeak))
			//{
			//  stateAngle = Math.Abs(stateAngle - 2.0 * Math.PI);
			//}

			//IPoint3D start = new Point3D();
			//IPoint3D end = new Point3D();
			//return GetCoordinates(segmentCount, ref start, ref end, ref sameSide);

			double diff = GetAngleDiff(segmentCount, angle);
			double diffN = GetAngleDiff(segmentCount, -angle);
			if (diffN < diff) // it can happen that angle is negative default
			{
				diff = diffN;
			}

			IPoint3D start = new Point3D();
			IPoint3D end = new Point3D();
			return GetCoordinates(segmentCount, ref start, ref end, ref sameSide) ? diff : -1;
		}

		/// <summary>
		/// Compute parabola updated angle.
		/// </summary>
		/// <param name="angle">Parabola Angle</param>
		/// <returns>Parabola Updated Angle</returns>
		private double ComputeParabolaAngle(double angle)
		{
			if (!((maximumResult >= 0) && (maximumResult <= ResultCount)))
			{
				throw new NotImplementedException();
			}

			double s = Math.Sin(-angle), c = Math.Cos(-angle);

			Vector3D a = (vectorCA * c) + (vectorNormalCA * s);
			Vector3D b = (vectorCB * c) + (vectorNormalCB * s);

			double nax = a | localX;
			double nay = a | localY;

			double nbx = b | localX;
			double nby = b | localY;

			double res = (nay * nbx * nbx) - (nby * nax * nax);

			for (int i = 0; i < maximumResult; i++)
			{
				if (!(angle - resultCollection[i]).IsZero(MathConstants.ZeroGeneral))
				{
					res /= angle - resultCollection[i];
				}
				else
				{
					//if (!((maximumResult >= 0) && (maximumResult < ResultCount)))
					//{
					//  throw new NotImplementedException();
					//}

					//maximumResult--;
					//res = (ComputeParabolaAngle(angle + MathConstants.ZeroGeneral) - res) / MathConstants.ZeroGeneral;
					//maximumResult++;

					//if (res.IsZero(MathConstants.ZeroGeneral))
					//{
					//  return res;
					//}

					res = ComputeParabolaAngle(angle + 1e-8/*MathConstants.ZeroGeneral*/) + ComputeParabolaAngle(angle - 1e-8/*MathConstants.ZeroGeneral*/);
					res /= 2.0;

					//if (!((maximumResult >= 0) && (maximumResult < ResultCount)))
					//{
					//  throw new NotImplementedException();
					//}
				}
			}

			return res;
		}

		/// <summary>
		/// Calculate Local Coordinate of parabola.
		/// </summary>
		/// <param name="segmentCount">Specifies the segment of parabola</param>
		/// <param name="startPoint">startpoint</param>
		/// <param name="endPoint">endpoint</param>
		/// <param name="sameSide">State of Parabola</param>
		/// <returns>true if parabola segment Valid</returns>
		private bool GetCoordinates(int segmentCount, ref IPoint3D startPoint, ref IPoint3D endPoint, ref bool sameSide)
		{
			double s = Math.Sin(-resultCollection[segmentCount]);
			double c = Math.Cos(-resultCollection[segmentCount]);
			Vector3D a = (vectorCA * c) + (vectorNormalCA * s);
			Vector3D b = (vectorCB * c) + (vectorNormalCB * s);

			startPoint.Y = a | localY;
			endPoint.Y = b | localY;

			// CIH - nevim, proč to tu je - to normálně funguje
			// if (startPoint.Y.IsLesser(0.0) || endPoint.Y.IsLesser(0.0))
			// {
			//	return false;
			// }
			startPoint.X = a | localX;
			endPoint.X = b | localX;
			//sameSide = startPoint.X.IsGreater(0.0) == endPoint.X.IsGreater(0.0);

			if (startPoint.Y < 0.0 || endPoint.Y < 0.0)
				return false;

			sameSide = (startPoint.X > 0) == (endPoint.X > 0);
			return true;
		}
	}
}
