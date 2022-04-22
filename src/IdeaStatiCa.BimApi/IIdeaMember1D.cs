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
	public interface IIdeaMember1D : IIdeaPersistentObject, IIdeaObjectWithResults
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
		///		<item>First element's <see cref="IIdeaElement1D.StartNode"/> is linked to the same node as <see cref="StartNode"/>.</item>
		///		<item>N-th element's <see cref="IIdeaElement1D.EndNode"/> is linked to the same node as (n+1)-th element's <see cref="IIdeaElement1D.StartNode"/>.</item>
		///		<item>Last element's <see cref="IIdeaElement1D.EndNode"/> is linked to the same node as <see cref="EndNode"/>.</item>
		/// </list>
		/// </para>
		/// </summary>
		List<IIdeaElement1D> Elements { get; }

		IIdeaTaper Taper { get; }

		IIdeaCrossSection CrossSection { get; }

		Alignment Alignment { get; }
	}
}