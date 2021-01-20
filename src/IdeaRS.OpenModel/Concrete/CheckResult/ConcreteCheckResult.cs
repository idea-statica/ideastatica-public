using System.Collections.Generic;

namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// Check result type
	/// </summary>
	public enum CheckResultType
	{
		/// <summary>
		/// calculation of capacity by the interaction diagram
		/// </summary>
		Capacity,

		/// <summary>
		/// calculation plane of deformation
		/// </summary>
		Response,

		/// <summary>
		/// shear check
		/// </summary>
		Shear,

		/// <summary>
		/// torsion check
		/// </summary>
		Torsion,

		/// <summary>
		/// interaction shear+torsion
		/// </summary>
		Interaction,

		/// <summary>
		/// calculation - response for fatigue
		/// </summary>
		Fatigue,

		/// <summary>
		/// stress limitation check
		/// </summary>
		StressLimitation,

		/// <summary>
		/// crack width check
		/// </summary>
		CrackWidth,

		/// <summary>
		/// detailing check
		/// </summary>
		Detailing,

		/// <summary>
		/// stiffness
		/// </summary>
		Stiffness,

		/// <summary>
		/// deflection
		/// </summary>
		Deflection
	}

	/// <summary>
	/// Concrete Check results
	/// </summary>
	public class ConcreteCheckResult
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConcreteCheckResult()
		{
			CheckResults = new List<ConcreteCheckResultBase>();
		}

		/// <summary>
		/// Check result type
		/// </summary>
		public CheckResultType ResultType { get; set; }

		/// <summary>
		/// All results - first is extreme
		/// </summary>
		public List<ConcreteCheckResultBase> CheckResults { get; set; }
	}
}