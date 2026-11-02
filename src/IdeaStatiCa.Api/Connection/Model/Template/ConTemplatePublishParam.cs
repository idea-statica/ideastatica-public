namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConTemplatePublishParam
	{
		public string Name { get; set; }
		public string Author { get; set; }
		public string CompanyName { get; set; }
		public ConDesignSetType DesignSetType { get; set; } = ConDesignSetType.Company;
	}

	public enum ConDesignSetType
	{
		Private,
		Company
	}
}
