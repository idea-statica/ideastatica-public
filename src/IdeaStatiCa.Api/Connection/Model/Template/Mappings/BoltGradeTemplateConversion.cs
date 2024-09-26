using System.Text.RegularExpressions;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class BoltGradeTemplateConversion : BaseTemplateConversion
	{
		public string FullBoltAssemblyName { get; set; }
		public BoltGradeTemplateConversion()
		{			
			//Description = IdeaStatica.Translation.Resources.Boltgrade;
		}
		//UltimateStrength
		public static string ParseGradeFromBoltAssemblyName(string boltAssemblyName)
		{
			string[] parts = Regex.Split(boltAssemblyName, @"\s+");
			if (parts.Length >= 2)
			{
				return parts[1];
			}
			else
			{
				return boltAssemblyName;
			}
		}
	}
}
