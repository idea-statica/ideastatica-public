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
			Debug.Assert(values.Count == src.ValueCountInSection, "Wrong dimensions");
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
		public static T GetValueInSection<T>(this ValuesInSegmentSections<T> src, int sectionInx, int loadingInx, int valInx) where T : struct
		{
			int sectionIndex = GetSectionValueIndex<T>(src, sectionInx, loadingInx);
			return src.Values[sectionIndex + valInx];
		}

		/// <summary>
		/// Calculates the index of a point values based on the specified loading index.
		/// </summary>
		/// <typeparam name="T">The type of the values in the point, which must be a value type.</typeparam>
		/// <param name="src">The source <see cref="ValuesInPoint{T}"/> instance containing the values.</param>
		/// <param name="loadingIdx">The loading index used to calculate the point value index. Must be a non-negative integer.</param>
		/// <returns>The calculated index of the point value within the section.</returns>
		public static int GetPointValueIndex<T>(this ValuesInPoint<T> src, int loadingIdx) where T : struct
		{
			return src.ValueCountInSection * loadingIdx;
		}

		/// <summary>
		/// Sets the values in a specified section of the <see cref="ValuesInPoint{T}"/> object.
		/// </summary>
		/// <remarks>This method updates the values in the specified section of the <paramref name="src"/> object,
		/// starting at the position determined by <paramref name="loadingIdx"/>. Ensure that the <paramref name="values"/>
		/// list contains enough elements to fill the section, as defined by the <see
		/// cref="ValuesInPoint{T}.ValueCountInSection"/> property.</remarks>
		/// <typeparam name="T">The type of the values, which must be a value type.</typeparam>
		/// <param name="src">The <see cref="ValuesInPoint{T}"/> instance where the values will be set.</param>
		/// <param name="loadingIdx">The index of the loading case that determines the starting position for setting values.</param>
		/// <param name="values">A list of values to set in the specified point.</param>
		public static void SetValuesInPoint<T>(this ValuesInPoint<T> src, int loadingIdx, IList<T> values) where T : struct
		{
			int loadCaseIndex = GetPointValueIndex(src, loadingIdx);
			for(int i = 0; i < src.ValueCountInSection; i++)
			{
				src.Values[loadCaseIndex + i] = values[i];
			}
		}

		/// <summary>
		/// Retrieves a value from the specified point in a collection of values.
		/// </summary>
		/// <typeparam name="T">The type of the value to retrieve, which must be a value type.</typeparam>
		/// <param name="src">The source collection of values from which to retrieve the value.</param>
		/// <param name="loadingIdx">The index representing the loading case in the collection.</param>
		/// <param name="valueIdx">The index of the specific value within the loading case to retrieve.</param>
		/// <returns>The value of type <typeparamref name="T"/> located at the specified indices.</returns>
		public static T GetValueInPoint<T>(this ValuesInPoint<T> src, int loadingIdx, int valueIdx) where T : struct
		{
			int loadCaseIndex = GetPointValueIndex(src, loadingIdx);
			return src.Values[loadCaseIndex + valueIdx];
		}
	}
}
