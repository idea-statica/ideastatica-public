using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// A system which uses one or more numbers, or coordinates, to uniquely determine the position of a point or other geometric element.
	/// </summary>
	public enum CoordinateSystemMethod
	{
		/// <summary>
		/// The Cartesian coordinate system.
		/// </summary>
		Cartesian,

		/// <summary>
		/// The polar coordinate system.
		/// </summary>
		Polar,
	}

	/// <summary>
	/// The symmetry type.
	/// </summary>
	public enum Symmetry
	{
		/// <summary>
		/// Symmetry is not available to input.
		/// </summary>
		NotAvailable,

		/// <summary>
		/// Symmetrical input.
		/// </summary>
		Symmetrical,

		/// <summary>
		/// Non-symmetrical input.
		/// </summary>
		Asymmetrical,
	}
}
