using IdeaRS.OpenModel.Geometry2D;
using System.Collections.Generic;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Point results
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class PointResults
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PointResults()
		{
			Results = new List<PointResultBase>();
		}

		/// <summary>
		/// Point
		/// </summary>
		public Point2D Point { get; set; }

		/// <summary>
		/// List of results in the point
		/// </summary>
		public List<PointResultBase> Results { get; set; }
	}
}