using System.Collections.Generic;

namespace IdeaStatiCa.Api.RCS.Model
{
	/// <summary>
	/// Cross-section geometry input
	/// </summary>
	public class RcsCrossSectionData
	{
		/// <summary>
		/// Name of the cross-section
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Cross-section rotation in radians
		/// </summary>
		public double CrossSectionRotation { get; set; }

		/// <summary>
		/// Geometry components of the cross-section
		/// </summary>
		public List<RcsCssComponentData> Components { get; set; } = new List<RcsCssComponentData>();
	}
}