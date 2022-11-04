namespace BimApiLinkFeaExample.FeaExampleApi
{
	public interface IFeaMember
	{
		int Id { get; }
		int BeginNode { get; }
		int EndNode { get; }
		int CrossSectionId { get; }
	}

	public class FeaMember : IFeaMember
	{
		public int Id { get; set; }
		public int BeginNode { get; set; }
		public int EndNode { get; set; }
		public int CrossSectionId { get; set; }
	}
}
