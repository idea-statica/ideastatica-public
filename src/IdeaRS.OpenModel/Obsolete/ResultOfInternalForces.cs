using System;
using System.Reflection;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Result of internal forces in the one position
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[Obsolete]
	public class ResultOfInternalForces2 : ResultOfLoading2
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOfInternalForces2()
		{
			N = Qy = Qz = Mx = My = Mz = 0.0;
		}

		/// <summary>
		/// LoadingTypeClass
		/// </summary>
		public Loading Loading { get; set; }

		/// <summary>
		/// Normal force
		/// </summary>
		public double N { get; set; }

		/// <summary>
		/// Shear force in y dirrection
		/// </summary>
		public double Qy { get; set; }

		/// <summary>
		/// Shear force in z dirrection
		/// </summary>
		public double Qz { get; set; }

		/// <summary>
		/// Bending moment around x-axis
		/// </summary>
		public double Mx { get; set; }

		/// <summary>
		/// Bending moment around y-axis
		/// </summary>
		public double My { get; set; }

		/// <summary>
		/// Bending moment around z-axis
		/// </summary>
		public double Mz { get; set; }
	}

	/// <summary>
	/// Result of internal forces in the one position
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[XmlInclude(typeof(ResultOfDeformation2))]
	[XmlInclude(typeof(ResultOfInternalForces2))]
	[XmlInclude(typeof(ResultOfNLA))]
	[Obsolete]
	public abstract class ResultOfLoading2
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOfLoading2()
		{
		}
	}
}