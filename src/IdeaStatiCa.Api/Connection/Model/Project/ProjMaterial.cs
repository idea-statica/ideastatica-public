namespace IdeaStatiCa.Api.Connection.Model
{
	public class ProjMaterial : ProjItem
	{
		public string MaterialType { get; set; }

		public string IsFromMrplDatabase { get; set; }

		public bool CanEdit { get; set; }
	}
}
