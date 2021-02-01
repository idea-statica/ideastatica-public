using System.Windows.Media;

namespace CI.Geometry2D
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. This rule causes build error.")]
	internal class Line2DTransform : ISegment2DTransform
	{
		public void Transform(ISegment2D source, ref Matrix matrix, out ISegment2D target)
		{
			target = new LineSegment2D(matrix.Transform(source.EndPoint));
		}
	}
}