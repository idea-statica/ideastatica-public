namespace BimApiLinkFeaExample.FeaExampleApi
{
	public interface IFeaMemberResult
	{
		int MemberId { get; }

		string ResultType { get; }

		int LoadCaseId { get; }
		
		/// <summary>
		/// Position of the result in direction of X axis of the member
		/// </summary>
		double Location { get; }

		/// <summary>
		/// Determination of the order of the result, 
		/// when there are two results in the same position
		/// </summary>
		int Index { get; }

		double N { get; }

		double Vy { get; }

		double Vz { get; }

		double Mx { get; }

		double My { get; }

		double Mz { get; }
	}

	internal class FeaMemberResult : IFeaMemberResult
	{
		public int MemberId { get; set; }

		public string ResultType { get; set; }

		public int LoadCaseId { get; set; }

		public double Location { get; set; }

		public int Index { get; set; }

		public double N { get; set; }

		public double Vy { get; set; }

		public double Vz { get; set; }

		public double Mx { get; set; }

		public double My { get; set; }

		public double Mz { get; set; }
	}
}
