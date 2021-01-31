using CI;
using CI.Geometry3D;
using CI.Mathematics;
using System;
using WM = System.Windows.Media.Media3D;

namespace IdeaRS.Connections.Commands
{
	public static partial class ConnectedBeamTools
	{
		static public WM.Point3D GlobalOrigin = new WM.Point3D(0, 0, 0);

		static public WM.Vector3D GlobalAxisX = new WM.Vector3D(1, 0, 0);
		static public Plane3D GlobalPlaneYZ = new Plane3D(GlobalOrigin, GlobalAxisX);

		static public WM.Vector3D GlobalAxisZ = new WM.Vector3D(0, 0, 1);
		static public Plane3D GlobalPlaneXY = new Plane3D(GlobalOrigin, GlobalAxisZ);

		static public WM.Vector3D GlobalAxisY = new WM.Vector3D(0, 1, 0);
		static public Plane3D GlobalPlaneXZ = new Plane3D(GlobalOrigin, GlobalAxisY);

		/// <summary>
		/// Gets angles of the connected beam
		/// </summary>
		/// <param name="dirVect">Direction vector</param>
		/// <param name="alpha">Angle from the plane XY</param>
		/// <param name="beta">Rotation around axis Z</param>
		public static void GetAngles(WM.Vector3D dirVect, out double alpha, out double beta)
		{
			const double precision = 1e-6;
			dirVect.Normalize();

			bool isXZero = dirVect.X.IsZero(precision);
			bool isYZero = dirVect.Y.IsZero(precision);
			bool isZZero = dirVect.Z.IsZero(precision);

			if (isYZero && isXZero)
			{
				alpha = 0.0;
				if (dirVect.Z > 0)
				{
					beta = MathConstants.PI_2;
				}
				else
				{
					beta = -MathConstants.PI_2;
				}
				return;
			}
			else if (isYZero && isZZero)
			{
				alpha = 0.0;
				if (dirVect.X > 0)
				{
					beta = 0;
				}
				else
				{
					alpha = MathConstants.PI;
					beta = 0;
				}
				return;
			}
			else if (isXZero && isZZero)
			{
				beta = 0.0;
				if (dirVect.Y > 0)
				{
					alpha = MathConstants.PI_2;
				}
				else
				{
					alpha = -MathConstants.PI_2;
				}
				return;
			}

			alpha = Math.Atan2(dirVect.Y, dirVect.X);
			if (dirVect.Y.IsZero())
			{
				alpha = dirVect.X.IsGreater(0) ? 0 : MathConstants.PI;
				beta = Math.Atan2(dirVect.Z, Math.Abs(dirVect.X));
			}
			else
			{
				double xy = Math.Sqrt(dirVect.X * dirVect.X + dirVect.Y * dirVect.Y);
				beta = Math.Atan2(dirVect.Z, xy);

				if (alpha.IsEqual(MathConstants.PI, precision) || (-alpha).IsEqual(MathConstants.PI, precision))
				{
					alpha = MathConstants.PI;
				}
			}
		}
	}
}