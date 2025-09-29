using IdeaRS.OpenModel.Model;
using System;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// An element is a part of a member. Every member is comprised of one or more elements.
	/// It's geometry is defined by a <see cref="Segment"/> that can be either a line or an arc. It must specify
	/// cross-sections at both ends.
	/// </summary>
	public interface IIdeaElement1D : IIdeaObjectWithResults
	{
		/// <summary>
		/// Cross-section at the start of the element.
		/// </summary>
		//[Obsolete("Element cross-section are obsolete, use member cross-section or member spans for haunched members.")]
		IIdeaCrossSection StartCrossSection { get; }

		/// <summary>
		/// Cross-section at the and of the element.
		/// </summary>
		//[Obsolete("Element cross-section are obsolete, use member cross-section or member spans for haunched members.")]
		IIdeaCrossSection EndCrossSection { get; }

		/// <summary>
		/// Rotation of the element around the x-axis of the <see cref="Segment"/>'s LCS.
		/// </summary>
		double RotationRx { get; }

		/// <summary>
		/// Returns the segment object that should implement either <see cref="IIdeaLineSegment3D"/> or <see cref="IIdeaArcSegment3D"/>. Must not return null.
		/// <para>Hint: use is-operator to determine the segment type.</para>
		/// </summary>
		IIdeaSegment3D Segment { get; }
	}
}