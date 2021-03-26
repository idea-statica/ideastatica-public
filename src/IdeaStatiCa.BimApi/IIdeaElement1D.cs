
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
		/// Cross-section at the start of the element
		/// </summary>
		IIdeaCrossSection StartCrossSection { get; }

		/// <summary>
		/// Cross-section at the and of the element
		/// </summary>
		IIdeaCrossSection EndCrossSection { get; }

		/// <summary>
		/// Eccentricity at the start of the element
		/// </summary>
		IdeaVector3D EccentricityBegin { get; }

		/// <summary>
		/// Eccentricity at the end of the element
		/// </summary>
		IdeaVector3D EccentricityEnd { get; }

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