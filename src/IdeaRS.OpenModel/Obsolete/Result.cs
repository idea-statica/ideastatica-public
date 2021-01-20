using IdeaRS.OpenModel.Geometry3D;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Common result
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[Obsolete]
	public class ResultInSection
	{
		/// <summary>
		/// Position
		/// </summary>
		public double Position { get; set; }

		/// <summary>
		/// Result In Section
		/// </summary>
		public List<ResultOfLoading2> ResultOfLoading { get; set; }
	}

	/// <summary>
	/// Point of results
	/// </summary>
	[Obsolete]
	public class ResultPoint
	{
		/// <summary>
		/// Id of Point
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Location of Point
		/// </summary>
		public Point3D Point { get; set; }
	}
}