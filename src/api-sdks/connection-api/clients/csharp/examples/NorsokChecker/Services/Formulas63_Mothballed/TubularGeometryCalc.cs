using NorsokChecker.Models;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Computes all geometric properties of a tubular (CHS) cross-section
	/// from outside diameter D and wall thickness t.
	/// Per NORSOK N-004 §6.3 notation.
	/// </summary>
	public static class TubularGeometryCalc
	{
		public static TubularGeometry Calculate(double D, double t)
		{
			double Di = D - 2 * t;

			double D2 = D * D;
			double Di2 = Di * Di;
			double D3 = D2 * D;
			double Di3 = Di2 * Di;
			double D4 = D2 * D2;
			double Di4 = Di2 * Di2;

			double A = Math.PI / 4.0 * (D2 - Di2);
			double Ix = Math.PI / 64.0 * (D4 - Di4);
			double Ip = Math.PI / 32.0 * (D4 - Di4);
			double W = Math.PI / 32.0 * (D4 - Di4) / D;  // = I / (D/2)
			double Z = (1.0 / 6.0) * (D3 - Di3);
			double i = Math.Sqrt(Ix / A);

			return new TubularGeometry
			{
				D = D,
				t = t,
				Di = Di,
				A = A,
				W = W,
				Z = Z,
				I = Ix,
				Ip = Ip,
				i = i,
				DtRatio = D / t,
				ZWRatio = Z / W
			};
		}
	}
}
