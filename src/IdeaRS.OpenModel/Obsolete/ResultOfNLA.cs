using System;
using System.Collections.Generic;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Result of internal forces in the one position
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[Obsolete]
	public class ResultOfNLA2 : ResultOfLoading2
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOfNLA2()
		{
			Points = new List<ResultOfNLAPoint>();
		}

		/// <summary>
		/// Results in section
		/// </summary>
		public List<ResultOfNLAPoint> Points { get; set; } // axial force
	}

	/// <summary>
	/// Result of internal forces in the one position
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[Obsolete]
	public class ResultOfNLAPoint2
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOfNLAPoint2()
		{
			ResultOfIncrements = new List<ResultOfIncrement2>();
		}

		/// <summary>
		/// Id of point
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// X coordinates on cross-section
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// Y coordinates on cross-section
		/// </summary>
		public double Y { get; set; }

		/// <summary>
		/// Results in load case
		/// </summary>
		public List<ResultOfIncrement2> ResultOfIncrements { get; set; } // axial force
	}

	/// <summary>
	/// Result of internal forces in the one position
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[Obsolete]
	public class ResultOfIncrement2
	{
		///// <summary>
		///// Id of load case
		///// </summary>
		//public int IdLoading { get; set; }

		///// <summary>
		///// Type of loading
		///// </summary>
		//public LoadingType TypeOfLoading { get; set; }

		/// <summary>
		/// Increment of load case
		/// </summary>
		public int Increment { get; set; }

		/// <summary>
		/// Axial force
		/// </summary>
		public double AxialForce { get; set; } // axial force

		/// <summary>
		/// Axial stress
		/// </summary>
		public double AxialStress { get; set; } // axial stress

		/// <summary>
		/// Axial strain
		/// </summary>
		public double AxialStrain { get; set; } // axial strain
	}
}