using System.Collections.Generic;
using System.Diagnostics;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Extensions for accessing values stored in  <see cref="IdeaRS.OpenModel.Result.ValuesInSegmentSections{T}"/>
	/// </summary>
	public static class Member2DReactExtensions
	{
		/// <summary>
		/// Get index of the value in <see cref="IdeaRS.OpenModel.Result.ValuesInSegmentSections{T}.Values"/> for given section and loading
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="src"></param>
		/// <param name="sectionInx">Index of the requested section in <see cref="IdeaRS.OpenModel.Result.ValuesInSegmentSections{T}.PositionsOnSegment"/>(0 is the first section (begin of the segment))</param>
		/// <param name="loadingInx">Index of the requested loading in <see cref="IdeaRS.OpenModel.Result.Member2DReactions{T}.Loadings"/></param>
		/// <returns></returns>
		public static int GetSectionValueIndex<T>(this ValuesInSegmentSections<T> src, int sectionInx, int loadingInx) where T : struct
		{
			int valuesInSect = src.ValueCountInSection * src.LoadingCount;
			int resIndex = sectionInx * valuesInSect + loadingInx * src.ValueCountInSection;
			return resIndex;
		}

		/// <summary>
		/// Set values in the section for loading
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="src"></param>
		/// <param name="sectionInx">Index of the section in <see cref="IdeaRS.OpenModel.Result.ValuesInSegmentSections{T}.PositionsOnSegment"/>(0 is the first section (begin of the segment))</param>
		/// <param name="loadingInx">Index of the requested loading in <see cref="IdeaRS.OpenModel.Result.Member2DReactions{T}.Loadings"/></param>
		/// <param name="values">List of values to set. The length must equal to <see cref="IdeaRS.OpenModel.Result.ValuesInSegmentSections{T}.ValueCountInSection"/></param>
		public static void SetValuesInSection<T>(this ValuesInSegmentSections<T> src, int sectionInx, int loadingInx, IList<T> values) where T : struct
		{
			Debug.Assert(values.Count != src.ValueCountInSection, "Wrong dimensions");
			int sectionIndex = GetSectionValueIndex<T>(src, sectionInx, loadingInx);
			for(int i = 0; i < src.ValueCountInSection; i++)
			{
				src.Values[sectionIndex + i] = values[i];
			}
		}

		/// <summary>
		/// Set value in the section for loading
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="src"></param>
		/// <param name="sectionInx">Index of the section in <see cref="IdeaRS.OpenModel.Result.ValuesInSegmentSections{T}.PositionsOnSegment"/>(0 is the first section (begin of the segment))</param>
		/// <param name="loadingInx">Index of the requested loading in <see cref="IdeaRS.OpenModel.Result.Member2DReactions{T}.Loadings"/></param>
		/// <param name="valInx">the value index (from 0 to <see cref="IdeaRS.OpenModel.Result.ValuesInSegmentSections{T}.ValueCountInSection"/>) - 1)</param>
		/// <returns></returns>
		public static T GetValue<T>(this ValuesInSegmentSections<T> src, int sectionInx, int loadingInx, int valInx) where T : struct
		{
			int sectionIndex = GetSectionValueIndex<T>(src, sectionInx, loadingInx);
			return src.Values[sectionIndex + valInx];
		}
	}
}
