using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Point Result of TA
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class PointResultOfTA : PointResultBase
	{
		/// <summary>
		/// Temperature
		/// </summary>
		public double Temp { get; set; }
	}
}