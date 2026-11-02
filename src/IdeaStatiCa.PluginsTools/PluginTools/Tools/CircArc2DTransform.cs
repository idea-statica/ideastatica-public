using System.Windows.Media;

namespace CI.Geometry2D
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. This rule causes build error.")]
	internal class CircArc2DTransform : ISegment2DTransform
	{
		public void Transform(ISegment2D source, ref Matrix matrix, out ISegment2D target)
		{
			CircularArcSegment2D segment = source as CircularArcSegment2D;
			target = new CircularArcSegment2D(matrix.Transform(segment.EndPoint), matrix.Transform(segment.Point));
		}
	}
}