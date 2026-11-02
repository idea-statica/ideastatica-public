using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Xml.Serialization;

namespace CI.Geometry2D
{
	/// <summary>
	/// Represents an x- and y-coordinate pair in two-dimensional space as visible to COM components.
	/// </summary>
	[Guid("73C75AF3-E194-4081-9F4F-9AA0C99B5BBD")]
	[ComVisible(true)]
	[Serializable, XmlInclude(typeof(Point))]
	[DebuggerDisplay("[{X}; {Y}]")]
	public struct IdaComPoint2D
	{
		public double X;
		public double Y;

		public IdaComPoint2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public static implicit operator Point(IdaComPoint2D point)
		{
			return new Point(point.X, point.Y);
		}

		public static implicit operator IdaComPoint2D(Point point)
		{
			return new IdaComPoint2D(point.X, point.Y);
		}

		/// <summary>
		/// Compares two <c>IdaComPoint2D</c> structures for equality.
		/// </summary>
		/// <param name="point1">The first <c>IdaComPoint2D</c> structure to compare.</param>
		/// <param name="point2">The second <c>IdaComPoint2D</c> structure to compare.</param>
		/// <returns>
		/// true if both the <c>IdaComPoint2D.X</c> and <c>IdaComPoint2D.Y</c> coordinates
		/// of point1 and point2 are equal; otherwise, false.
		/// </returns>
		public static bool operator ==(IdaComPoint2D point1, IdaComPoint2D point2)
		{
			return point1.Equals(point2);
		}

		/// <summary>
		/// Compares two <c>IdaComPoint2D</c> structures for inequality.
		/// </summary>
		/// <param name="point1">The first <c>IdaComPoint2D</c> structure to compare.</param>
		/// <param name="point2">The second <c>IdaComPoint2D</c> structure to compare.</param>
		/// <returns>
		/// true if point1 and point2 have different <c>IdaComPoint2D.X</c> or <c>IdaComPoint2D.Y</c> coordinates;
		/// false if point1 and point2 have the same <c>IdaComPoint2D.X</c>
		/// and <c>IdaComPoint2D.Y</c> coordinates.
		/// </returns>
		public static bool operator !=(IdaComPoint2D point1, IdaComPoint2D point2)
		{
			return !point1.Equals(point2);
		}

		/// <summary>
		/// Compares two <c>IdaComPoint2D</c> structures for equality.
		/// Note: <c>IdaComPoint2D</c>
		///     coordinates are expressed using Double values. Because the value of a System.Double
		///     can lose precision when operated on, a comparison between two Doubles that
		///     are logically equal might fail.
		/// </summary>
		/// <param name="o">The point to compare to this instance.</param>
		/// <returns>
		/// true if both <c>IdaComPoint2D</c> structures contain the same <c>IdaComPoint2D.X</c>
		/// and <c>IdaComPoint2D.Y</c> values; otherwise, false.
		/// </returns>
		public override bool Equals(object o)
		{
			if (o is IdaComPoint2D)
			{
				var pt = (IdaComPoint2D)o;

				return this.X.Equals(pt.X) && this.Y.Equals(pt.Y);
			}

			throw new ArgumentException("object o is not a type of IdaComPoint2D");
		}

		/// <summary>
		/// Returns the hash code for this <c>IdaComPoint2D</c>.
		/// </summary>
		/// <returns>The hash code for this <c>IdaComPoint2D</c> structure.</returns>
		public override int GetHashCode()
		{
			return (int)X ^ (int)Y;
		}

		/// <summary>
		/// // scalar multiply of 2D vectors 
		/// </summary>
		/// <param name="v1">first vector</param>
		/// <param name="v2">second vector</param>
		/// <returns></returns>
		public static double operator |(IdaComPoint2D v1, IdaComPoint2D v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y;
		}

		/// <summary>
		/// projection point to vector direction 
		/// </summary>
		/// <param name="nDir">direction of projection</param>
		/// <returns>length of projection</returns>
		public double Projection(IdaComPoint2D nDir)
		{
			double projection = this | nDir;
			return projection;
		}

		/// <summary>
		/// projection point to vector direction 
		/// </summary>
		/// <param name="point">point</param>
		/// <param name="nDir">direction of projection</param>
		/// <returns>length of projection</returns>
		public static double Projection(Point point, IdaComPoint2D nDir)
		{
			double projection = point | nDir;
			return projection;
		}


		/// <summary>
		/// size of vector 
		/// </summary>
		/// <param name="vector">input vector</param>
		/// <returns></returns>
		public static double operator ~(IdaComPoint2D vector)
		{
			return Math.Sqrt(vector | vector);
		}

		/// <summary>
		/// vector division 
		/// </summary>
		/// <param name="vector">vector</param>
		/// <param name="r">value</param>
		/// <returns></returns>
		public static IdaComPoint2D operator /(IdaComPoint2D vector, double r)
		{
			return new IdaComPoint2D(vector.X / r, vector.Y / r);
		}

		/// <summary>
		/// convert to normal vector 
		/// </summary>
		/// <returns>normal vector</returns>
		public static IdaComPoint2D operator !(IdaComPoint2D vector)
		{
			double norm = ~vector;
			if (norm == 0)
			{
				return vector;
			}
			else
			{
				return new IdaComPoint2D(vector.X / norm, vector.Y / norm);
			}
		}

		/// <summary>
		/// minus operator 
		/// </summary>
		/// <param name="v1">first vector</param>
		/// <param name="v2">second vector</param>
		/// <returns>return IdaComPoint2D</returns>
		// 		public static IdaComPoint2D operator -(IdaComPoint2D v1, IdaComPoint2D v2)
		// 		{
		// 			return new IdaComPoint2D(v1.X - v2.X, v1.Y - v2.Y);
		// 		}

		/// <summary>
		/// plus operator 
		/// </summary>
		/// <param name="v1">first vector</param>
		/// <param name="v2">second vector</param>
		/// <returns>return IdaComPoint2D</returns>
		public static IdaComPoint2D operator +(IdaComPoint2D v1, IdaComPoint2D v2)
		{
			return new IdaComPoint2D(v1.X + v2.X, v1.Y + v2.Y);
		}

		/// <summary>
		/// plus operator 
		/// </summary>
		/// <param name="v1">first vector</param>
		/// <param name="v2">second vector</param>
		/// <returns>return IdaComPoint2D</returns>
		public static IdaComPoint2D operator +(IdaComPoint2D v1, Vector v2)
		{
			return new IdaComPoint2D(v1.X + v2.X, v1.Y + v2.Y);
		}

		/// <summary>
		/// minus operator 
		/// </summary>
		/// <param name="v1">first vector</param>
		/// <param name="v2">second vector</param>
		/// <returns>return IdaComPoint2D</returns>
		public static IdaComPoint2D operator -(IdaComPoint2D v1, Vector v2)
		{
			return new IdaComPoint2D(v1.X - v2.X, v1.Y - v2.Y);
		}

		/// <summary>
		/// multiply operator 
		/// </summary>
		/// <param name="v">vector</param>
		/// <param name="r">value</param>
		/// <returns></returns>
		public static IdaComPoint2D operator *(IdaComPoint2D v, double r)
		{
			return new IdaComPoint2D(v.X * r, v.Y * r);
		}

		/// <summary>
		/// calculation distance between two points
		/// </summary>
		/// <param name="v1">first point</param>
		/// <param name="v2">second point</param>
		/// <returns>length between these two points</returns>
		public static double VectorLength(Point v1, Point v2)
		{
			Vector v12 = v1 - v2;
			return v12.Length;
		}

		public double DistancePointToVector(Point v1, Point v2)
		{
			Vector AB = v2 - v1;
			Vector AP = this - v1;

			IdaComPoint2D v12 = new IdaComPoint2D(AB.X, AB.Y);
			IdaComPoint2D v1p = new IdaComPoint2D(AP.X, AP.Y);

			//AB nesmi byt nulovy vektor, ten by vsak nedefinoval primku
			double t = (v12 | v1p) / (v12 | v12);

			// to je ten kyzeny bod na primce AB
			IdaComPoint2D K = v1 + v12 * t;

			double Dist = VectorLength(K, this);

			//vzdalenost prominuteho bodu K na primky AB od bodu A
			double Dist1 = VectorLength(K, v1);

			//vzdalenost prominuteho bodu K na primky AB od bodu B
			double Dist2 = VectorLength(K, v2);

			//delka usecky AB	
			double Dist3 = VectorLength(v1, v2);

			//vzdalenost vlozky od zacatku usecky 
			double Dist4 = VectorLength(this, v1);

			//vzdalenost vlozky od konce usecky 
			double Dist5 = VectorLength(this, v2);

			//v pripade, ze soucet  Dist1+Dis2 je roven Dist3 potom bod lezi mezi 
			//body AB a nejkrati vzdalenost je Dist, v opacnem pripade je to jedna 
			//ze vzdalenosti Dist4 nebo Dist5
			if ((Dist3 - Dist2 - Dist1).IsZero())
			{
				return Dist;
			}
			else
			{
				if (Dist4 > Dist5) return Dist5;
				else return Dist4;
			}
		}
	}
}
