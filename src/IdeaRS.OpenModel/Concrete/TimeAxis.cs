using System.Collections.Generic;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Point on time axis
	/// </summary>
	public class TimePoint : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public TimePoint()
		{
		}

		/// <summary>
		/// Name of time point
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Time
		/// </summary>
		public double Time { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Stage
		/// </summary>
		public bool Stage { get; set; }

		/// <summary>
		/// Age
		/// </summary>
		public double Age { get; set; }

		/// <summary>
		/// Prestressing
		/// </summary>
		public bool Prestressing { get; set; }

		/// <summary>
		/// Last construction stage - Time of the start of cyclic loading for fatigue check
		/// </summary>
		public bool LastConstructionStage { get; set; }

		/// <summary>
		/// true to allow chec in age less 3 days
		/// </summary>
		public bool AllowCheckInAgeLess3Days { get; set; }

		/// <summary>
		/// true to fix around Y axis ( Mz is not taken into account )
		/// </summary>
		public bool Fix2DRotation { get; set; }
	}

	/// <summary>
	/// Time axis
	/// </summary>
	public class TimeAxis : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public TimeAxis()
		{
			Times = new List<TimePoint>();
			CreepTime = double.NaN;
		}

		/// <summary>
		/// Times
		/// </summary>
		public List<TimePoint> Times { get; set; }

		/// <summary>
		/// time from which the creep is calculated
		/// </summary>
		public double CreepTime { get; set; }
	}
}