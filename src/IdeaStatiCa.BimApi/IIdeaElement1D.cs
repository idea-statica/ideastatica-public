
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Element is part of member, that consists of a line or arc segment with different cross-sections and materials at the beginning and end.
	/// </summary>
	public interface IIdeaElement1D : IIdeaObject
	{
		/// <summary>
		/// Start node of the element. Returns null if unconnected. Is not equal to <see cref="EndNode"/>.
		/// </summary>
		IIdeaNode StartNode { get; }

		/// <summary>
		/// End node of the element. Returns null if unconnected. Is not equal to <see cref="StartNode"/>.
		/// </summary>
		IIdeaNode EndNode { get; }

		/// <summary>
		/// Cross-section at the start of the element
		/// </summary>
		IIdeaCrossSection StartCrossSection { get; }

		/// <summary>
		/// Cross-section at the and of the element
		/// </summary>
		IIdeaCrossSection EndCrossSection { get; }

		/// <summary>
		/// Excentricity at the start of the element
		/// </summary>
		double ExcentricityBegin { get; }

		/// <summary>
		/// Excentricity at the end of the element
		/// </summary>
		double ExcentricityEnd { get; }

		/// <summary>
		/// Rotation of the element around the x-axis
		/// </summary>
		double RotationRx { get; }

		/// <summary>
		/// Returns the segment object that should implement either <see cref="IIdeaLineSegment3D"/> or <see cref="IIdeaArcSegment3D"/>. Must not return null.
		/// <para>Hint: use is-operator to determine the segment type.</para>
		/// <para>Segments's <see cref="IIdeaSegment3D.StartNode"/> is connected to <see cref="StartNode"/> and segments's <see cref="IIdeaSegment3D.EndNode"/> is connected to <see cref="EndNode"/></para>
		/// </summary>
		IIdeaSegment3D Segment { get; }

	}
}