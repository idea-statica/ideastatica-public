using System.Windows.Media;

namespace CI.Geometry2D
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. This rule causes build error.")]
	interface ISegment2DTransform
	{
		void Transform(ISegment2D source, ref Matrix matrix, out ISegment2D target);
	}
}
