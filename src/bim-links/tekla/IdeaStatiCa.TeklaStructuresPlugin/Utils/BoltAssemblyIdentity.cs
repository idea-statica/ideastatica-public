using System.Globalization;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utils
{
	/// <summary>
	/// Builds the identity of a bolt assembly from the values that define it, so that equal assemblies share one
	/// identity and unequal ones never do. Every value an assembly does not derive from these three must be added here.
	/// </summary>
	public static class BoltAssemblyIdentity
	{
		/// <param name="diameter">In meters, as the raw value. A diameter already rendered for display is not
		/// interchangeable with it: <c>MetersToInchesFormated</c> drops any fractional inch it cannot match to one of
		/// eight common fractions, so 0.024 and 0.025 both render as <c>0"</c>.</param>
		public static string Create(string assemblyName, string boltGrade, double diameter)
		{
			return $"{assemblyName}|{boltGrade}|{diameter.ToString("G17", CultureInfo.InvariantCulture)}";
		}
	}
}
