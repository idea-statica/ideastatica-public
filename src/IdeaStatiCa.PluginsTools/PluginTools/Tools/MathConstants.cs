using System.Diagnostics.CodeAnalysis;

namespace CI.Mathematics
{
	/// <summary>
	/// Common mathematic constants
	/// </summary>
	public static class MathConstants
	{
		#region Members

		/// <summary>
		/// Magic constant
		/// </summary>
		public const double MagicConstant = 0.371382;

		/// <summary>
		/// Reverse angle
		/// </summary>
		public const double ReverseAngle = 180.0;

		/// <summary>
		/// Uses for general comparison
		/// </summary>
		public const double ZeroGeneral = 1e-10;

		/// <summary>
		/// The best comparison for zero
		/// </summary>
		public const double ZeroMaximum = 1e-14;

		/// <summary>
		/// For checking limits (especially in iterations)
		/// </summary>
		public const double ZeroWeak = 1e-4;

		/// <summary>
		/// More precise definition of PI
		/// </summary>
		public const double PI = 3.14159265358979323846;

		/// <summary>
		/// More precise definition of 0.5*PI
		/// </summary>
		[SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
		public const double PI_2 = 1.57079632679489661923;

		/// <summary>
		/// More precise of definition of 0.25*PI
		/// </summary>
		[SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
		public const double PI_4 = 0.785398163397448309616;

		/// <summary>
		/// Acceleration of gravity 
		/// </summary>
		[SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1303:ConstFieldNamesMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
		public const double gn = 9.80665;

		#endregion
	}
}
