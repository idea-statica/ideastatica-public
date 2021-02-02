using System.Collections.Generic;

namespace CI.Geometry2D
{
	/// <summary>
	/// Main geometry discretization interface.
	/// </summary>
	public interface IDiscretizator
	{
		/// <summary>
		/// Discretize of exact geometry to discrete model.
		/// </summary>
		/// <param name="source">The precise object to discretization.</param>
		/// <param name="target">The destination.</param>
		/// <param name="targetInxs">The list of indexes as destination.</param>
		void Discretize(IPolyLine2D source, ref IPolygon2D target, IList<int> targetInxs);
	}
}
