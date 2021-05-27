using IdeaRS.OpenModel.Result;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi.Results
{
	/// <summary>
	/// Result on <see cref="IIdeaMember1D"/> or <see cref="IIdeaElement1D"/>.
	/// </summary>
	public interface IIdeaResult
	{
		/// <summary>
		/// Type of the coordinate system the results are defined in.
		/// </summary>
		ResultLocalSystemType CoordinateSystemType { get; }

		/// <summary>
		/// Sections with results.
		/// </summary>
		IEnumerable<IIdeaSection> Sections { get; }
	}
}