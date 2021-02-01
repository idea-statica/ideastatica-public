using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CI.Geometry2D
{
	public static class PolyLine2DTransform
	{
		#region Fields

		private static Dictionary<Type, ISegment2DTransform> transformators = new Dictionary<Type, ISegment2DTransform>(2);

		#endregion Fields

		static PolyLine2DTransform()
		{
			transformators.Add(typeof(LineSegment2D), new Line2DTransform());
			transformators.Add(typeof(CircularArcSegment2D), new CircArc2DTransform());
		}

		public static PolyLine2D Transform(IPolyLine2D polyline, ref Matrix matrix)
		{
			PolyLine2D transformedPolyline = new PolyLine2D(polyline.Segments.Count) { Id = polyline.Id };
			transformedPolyline.StartPoint = matrix.Transform(polyline.StartPoint);

			IList<ISegment2D> segments = transformedPolyline.Segments;

			Point start = polyline.StartPoint;
			foreach (var segment in polyline.Segments)
			{
				try
				{
					ISegment2D transformedSegment;
					SelectSegment2DTransformator(segment.GetType()).Transform(segment, ref matrix, out transformedSegment);
					segments.Add(transformedSegment);
				}
				catch (ArgumentOutOfRangeException e)
				{
					System.Diagnostics.Debug.Assert(false, e.ToString());
				}
				finally
				{
					start = segment.EndPoint;
				}
			}

			return transformedPolyline;
		}

		/// <summary>
		/// The selection of a segment transformator.
		/// </summary>
		/// <param name="type">The type of a segment to the transformations.</param>
		/// <returns>The object, which transform a segment.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Bad transformator type.</exception>
		private static ISegment2DTransform SelectSegment2DTransformator(Type type)
		{
			ISegment2DTransform transformator;
			transformators.TryGetValue(type, out transformator);
			if (transformator == null)
			{
				throw new ArgumentOutOfRangeException(string.Format("PolyLine2DTransform.SelectSegment2DTransformator(Type {0}) - transformator is null", type.Name));
			}

			return transformator;
		}
	}
}