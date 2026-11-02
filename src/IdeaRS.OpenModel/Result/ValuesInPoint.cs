using System.Collections.Generic;

namespace IdeaRS.OpenModel.Result
{
	public class ValuesInPoint<T> where T : struct
	{
		public ValuesInPoint()
		{
			Values = new List<T>();
		}

		public ValuesInPoint(int loadingCount, int valuesInPoint)
		{
			LoadingCount = loadingCount;
			ValueCountInSection = valuesInPoint;
			Values = new List<T>(new T[loadingCount * valuesInPoint]);
		}

		/// <summary>
		/// The identifier of the node in the original FEA model.
		/// It will be used for merging the reactions from more walls that meet in this node.
		/// </summary>
		public string NodeOriginalId { get; set; }

		/// <summary>
		/// Absolute coordinate X in global coordinate system 
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// Absolute coordinate Y in global coordinate system 
		/// </summary>
		public double Y { get; set; }

		/// <summary>
		/// Absolute coordinate Z in global coordinate system 
		/// </summary>
		public double Z { get; set; }

		/// <summary>
		/// Number of stored loading types 
		/// </summary>
		public int LoadingCount { get; set; }

		/// <summary>
		/// The number of values which are stored in the section for each loading
		/// </summary>
		public int ValueCountInSection { get; set; }

		/// <summary>
		/// Values in the point for each loading
		/// </summary>
		public List<T> Values { get; set; }
	}
}
