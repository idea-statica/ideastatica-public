using System;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Result of deformation in the one positon
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[Obsolete]
	public class ResultOfDeformation2 : ResultOfLoading2
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOfDeformation2()
		{
			Ux = Uy = Uz = Fix = Fiy = Fiz = 0.0;
		}

		/// <summary>
		/// LoadingTypeClass
		/// </summary>
		public Loading Loading { get; set; }

		/// <summary>
		/// Deformation in the x direction
		/// </summary>
		public double Ux { get; set; }

		/// <summary>
		/// Deformation in the y direction
		/// </summary>
		public double Uy { get; set; }

		/// <summary>
		/// Deformation in the z direction
		/// </summary>
		public double Uz { get; set; }

		/// <summary>
		/// Rotation around x-axis
		/// </summary>
		public double Fix { get; set; }

		/// <summary>
		/// Rotation  around y-axis
		/// </summary>
		public double Fiy { get; set; }

		/// <summary>
		/// Rotation  around z-axis
		/// </summary>
		public double Fiz { get; set; }
	}
}