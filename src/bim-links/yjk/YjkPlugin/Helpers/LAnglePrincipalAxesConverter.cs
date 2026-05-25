using IdeaRS.OpenModel.CrossSection;
using System;
using System.Linq;

namespace yjk.Helpers
{
	/// <summary>
	/// Converts L-angle internal forces from principal axes (u, v) — as returned by YJK —
	/// to YJK local axes (y = horizontal left, z = vertical up).
	/// After conversion, the standard YJK→IDEA sign corrections apply as for any other member.
	/// </summary>
	public static class LAnglePrincipalAxesConverter
	{
		/// <summary>
		/// Computes the principal axis angle α (radians) in YJK's frame (Y points left).
		/// Works for both equal-leg (B == D) and unequal-leg (B != D) angles.
		/// Fillet radii are ignored — thin-walled approximation.
		/// </summary>
		/// <param name="B">Horizontal leg width (m)</param>
		/// <param name="D">Vertical leg height (m)</param>
		/// <param name="t">Leg thickness (m)</param>
		/// <returns>α in YJK's frame, in radians</returns>
		public static double ComputePrincipalAngle(double B, double D, double t)
		{
			// Two rectangular sub-areas, measured from the outer corner (bottom-left)
			double A_vert  = (D - t) * t;   // vertical leg, excluding the corner square
			double A_horiz = B * t;          // horizontal leg (full width)
			double A_total = A_vert + A_horiz;

			// Centroid distances from outer corner
			double zy = (A_vert * (t / 2.0) + A_horiz * (B / 2.0)) / A_total;
			double zz = (A_vert * ((D - t) / 2.0 + t) + A_horiz * (t / 2.0)) / A_total;

			// Second moments of area about centroidal axes
			// Vertical leg centroid is at (D+t)/2 from bottom, horizontal leg centroid at t/2
			double Iy = (t * Math.Pow(D - t, 3) / 12.0) + A_vert * Math.Pow((D + t) / 2.0 - zz, 2)
					  + (B * Math.Pow(t, 3) / 12.0)      + A_horiz * Math.Pow(t / 2.0 - zz, 2);

			double Iz = ((D - t) * Math.Pow(t, 3) / 12.0) + A_vert * Math.Pow(t / 2.0 - zy, 2)
					  + (t * Math.Pow(B, 3) / 12.0)        + A_horiz * Math.Pow(B / 2.0 - zy, 2);

			// Product moment of area (non-zero for unsymmetric L-angle)
			double Iyz = A_vert  * (t / 2.0 - zy) * ((D + t) / 2.0 - zz)
					   + A_horiz * (B / 2.0 - zy)  * (t / 2.0 - zz);

			// Negate Iyz because YJK's local Y points left: mirroring Y flips the sign of the product of inertia
			return 0.5 * Math.Atan2(-2.0 * Iyz, Iy - Iz);
		}

		/// <summary>
		/// Rotates internal forces from principal axes (u, v) to local axes (y, z).
		/// Mv is negated before rotation because u/v point outward from the centroid: by right-hand rule,
		/// positive Mv produces tension on the +z side, opposite to the standard rotation convention.
		///   My =  Mu*cos + Mv*sin
		///   Mz =  Mu*sin - Mv*cos
		/// </summary>
		public static (double My, double Mz, double Vy, double Vz) ToLocalAxes(
			double Mu, double Mv, double Vu, double Vv, double alpha)
		{
			double cos = Math.Cos(alpha);
			double sin = Math.Sin(alpha);

			double My =  Mu * cos - Mv * sin * -1;
			double Mz =  Mu * sin + Mv * cos * -1;
			double Vy =  Vu * cos - Vv * sin;
			double Vz =  Vu * sin + Vv * cos;

			return (My, Mz, Vy, Vz);
		}

		/// <summary>
		/// Extracts a named ParameterDouble value from a CrossSectionParameter's Parameters list.
		/// Throws if the parameter is not found.
		/// </summary>
		public static double GetParameter(CrossSectionParameter css, string name)
		{
			var param = css.Parameters.OfType<ParameterDouble>().FirstOrDefault(p => p.Name == name);
			if (param == null)
				throw new InvalidOperationException($"LAnglePrincipalAxesConverter: parameter '{name}' not found in cross section parameters.");
			return param.Value;
		}
	}
}
