namespace BimApiLinkFeaExample.FeaExampleApi
{
	public enum LoadGroupCategory
	{ 
		Permanent,
		Variable
	}

	public interface IFeaLoadGroup
	{
		int Id { get; }
		string Name { get; }
		LoadGroupCategory LoadGroupCategory { get; }		
	}

	internal class FeaLoadGroup : IFeaLoadGroup
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public LoadGroupCategory LoadGroupCategory { get; set; }
	}
}
