using System.Collections.Generic;

namespace IdeaRS.OpenModel.Result
{
	public class ValuesInPoint<T> where T : struct
	{
		public ValuesInPoint()
		{
			Values = new List<T>();
		}

		public ValuesInPoint(int pointId, (double x, double y, double z) coordinates, int loadingCount, int valuesInPoint)
		{
			PointId = pointId;
			Coordinates = coordinates;
			LoadingCount = loadingCount;
			ValueCountInSection = valuesInPoint;
			Values = new List<T>(loadingCount * valuesInPoint);
		}

		/// <summary>
		/// ID of the point unique within the inner points of imported area
		/// </summary>
		public int PointId { get; set; }

		/// <summary>
		/// Absolute cooordinates in global coordinate system 
		/// </summary>
		public (double x, double y, double z) Coordinates { get; set; }

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
