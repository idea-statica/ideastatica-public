using System.Collections.Generic;
using System.Reflection;
using IdeaRS.OpenModel.Geometry2D;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Section result of points
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class SectionResultOnPoints : SectionResultBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SectionResultOnPoints()
		{
			Points = new List<PointResults>();
		}

		/// <summary>
		/// Section points
		/// </summary>
		public List<PointResults> Points { get; set; }
	}

	/// <summary>
	/// Section result mesh
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class SectionResultMesh : SectionResultBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SectionResultMesh()
		{
			Points = new List<Point2D>();
			Elements = new List<MeshElement2D>();
		}

		/// <summary>
		/// Section points
		/// </summary>
		public List<Point2D> Points { get; set; }

		/// <summary>
		/// Elements
		/// </summary>
		public List<MeshElement2D> Elements { get; set; }
	}

	/// <summary>
	/// Element2D
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class MeshElement2D
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MeshElement2D()
		{
			Points = new List<int>();
		}

		/// <summary>
		/// Points
		/// </summary>
		public List<int> Points { get; set; }
	}
}