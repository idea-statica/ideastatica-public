using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Represents values on <see cref="IdeaRS.OpenModel.Geometry3D.Segment3D"/>
	/// There are sections on the segment. In the section there can be stored N values for each loading (LoadCase, LoadCombination ..)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Obfuscation(Feature = "renaming")]
	public class ValuesInSegmentSections<T> where T : struct
	{
		public ValuesInSegmentSections()
		{
			PositionsOnSegment = new List<double>();
			Values = new List<T>();
		}

		/// <summary>
		/// Initialize the instance and allocate memory for requested sections, loading and required values in sections to be stored
		/// </summary>
		/// <param name="sectionCount"></param>
		/// <param name="loadingCount"></param>
		/// <param name="valuesInSection"></param>
		public ValuesInSegmentSections(int sectionCount, int loadingCount, int valuesInSection)
		{
			Type = "IdeaRS.OpenModel.Geometry3D.LineSegment3D";
			SectionCount = sectionCount;
			LoadingCount = loadingCount;
			ValueCountInSection = valuesInSection;
			PositionsOnSegment = new List<double>(sectionCount);
			int valCount = sectionCount * loadingCount * valuesInSection;
			Values = new List<T>(new T[valCount]); // Allocate array with required range
			NodeIdOnSegment = new List<string>(new string[sectionCount]);
		}

		/// <summary>
		/// Id if the segment in <see cref="OpenModel"/>
		/// </summary>
		public int SegmentId { get; set; }

		/// <summary>
		/// Type of the segment in <see cref="OpenModel"/>
		/// It can be <see cref="IdeaRS.OpenModel.Geometry3D.LineSegment3D"/> or <see cref="IdeaRS.OpenModel.Geometry3D.ArcSegment3D"/>
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Positions of the sections along the segment
		/// </summary>
		public List<double> PositionsOnSegment { get; set; }

		/// <summary>
		/// The identifier of the mesh node ID in the original FEA model.
		/// It will be used for merging the reactions from more walls that meet in this node.
		/// </summary>
		public List<string> NodeIdOnSegment { get; set; }

		/// <summary>
		/// The number of sections on the segment
		/// </summary>
		public int SectionCount { get; set; }

		/// <summary>
		/// Number of stored loading types 
		/// </summary>
		public int LoadingCount { get; set; }

		/// <summary>
		/// The number of values which are stored in the section for each loading
		/// </summary>
		public int ValueCountInSection { get; set; }

		/// <summary>
		/// Raw array of values.
		/// The dimension of the array is defined by SectionCount*LoadingCount*ValueCountInSection
		/// </summary>
		public List<T> Values { get; set; }
	}
}
