using IdeaRS.OpenModel.Model;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represents named and identifiable generic member of that connects to up to two end nodes and might go through other nodes.
	/// <para>
	/// Member is used to represent beam, column, truss, rib or beamslab
	/// </para>
	/// <para>
	/// Members consist from one or more elements, each defining cros-section and materials.</para>
	/// </summary>
	public interface IIdeaMember1D : IIdeaObjectConnectable, IIdeaObjectWithResults
	{
		/// <summary>
		/// Type of member: beam, column, truss, rib or beamslab.
		/// </summary>
		Member1DType Type { get; }

		/// <summary>
		/// Elements this member consists of. Always returns a valid list with at least one element.
		/// <para>
		/// The following guarantees are valid for the elements:
		/// <list type="bullet">
		///		<item>There is always at least one element in each member.</item>
		///		<item>The first <see cref="Element1D"/>'s <see cref="IIdeaSegment3D"/>'s <see cref="IIdeaSegment3D.StartNode"/> defines the start node of the member.</item>
		///		<item>The N-th <see cref="Element1D"/>'s <see cref="IIdeaSegment3D"/> <see cref="IIdeaSegment3D.EndNode"/> is linked to the same node as (n+1)-th element's <see cref="IIdeaSegment3D.StartNode"/>.</item>
		///		<item>The Last <see cref="Element1D"/>'s <see cref="IIdeaSegment3D"/> <see cref="IIdeaSegment3D.EndNode"/> defines the end node of the member.</item>
		/// </list>
		/// </para>
		/// </summary>
		List<IIdeaElement1D> Elements { get; }

		/// <summary>
		/// Taper for specifying haunched member.
		/// </summary>
		IIdeaTaper Taper { get; }

		/// <summary>
		/// Cross-section of the member.
		/// </summary>
		IIdeaCrossSection CrossSection { get; }

		/// <summary>
		/// Alignment of the member's cross-section. Eccentricities are added up to the alignment.
		/// </summary>
		Alignment Alignment { get; }

		/// <summary>
		/// Mirrors the cross-section of this member on the Y-axis.
		/// </summary>
		bool MirrorY { get; }

		/// <summary>
		/// Mirrors the cross-section of this member on the Z-axis.
		/// </summary>
		bool MirrorZ { get; }

		/// <summary>
		/// Eccentricity (offset) at the start of the element.
		/// It is defined in the local coordinate system.
		/// </summary>
		IdeaVector3D EccentricityBegin { get; }

		/// <summary>
		/// Eccentricity (offset) at the end of the element.
		/// It is defined in the local coordinate system.
		/// </summary>
		IdeaVector3D EccentricityEnd { get; }

		InsertionPoints InsertionPoint { get; }

		EccentricityReference EccentricityReference { get; }
	}
}