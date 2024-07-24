namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project
{
	public class ProjCrossSection : ProjItem
	{
		public string CrossSectionType { get; set; }

		public bool IsFromDatabase { get; set; }

		public string DatabaseName { get; set; }

		public string Material { get; set; }
	}
}
