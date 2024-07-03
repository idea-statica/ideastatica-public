using System.Text.RegularExpressions;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Template
{
	public class BoltAssemblyTemplateConversion : BaseTemplateConversion
	{
		public string FullBoltAssemblyName { get; set; }
		public BoltAssemblyTemplateConversion()
		{	
			//Description = IdeaStatica.Translation.Resources.Bolt;
		}

		public static string ParseDiameterFromBoltAssemblyName(string boltAssemblyName)
		{
			string[] parts = Regex.Split(boltAssemblyName, @"\s+");
			if (parts.Length >= 2)
			{
				return parts[0];
			}
			else
			{
				return boltAssemblyName;
			}
		}
		//BoltDiameter
	}
}
