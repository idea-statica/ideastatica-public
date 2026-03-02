namespace yjk.FeaApis
{
	public interface IFeaMember
	{
		int Id { get; }
		int BeginNodeId { get; }
		int EndNodeId { get; }
		int CrossSectionId { get; }
	}

	public class FeaMember : IFeaMember
	{
		public FeaMember(int id, int beginNodeId, int endNodeId, int crossSectionId)
		{
			Id = id;
			BeginNodeId = beginNodeId;
			EndNodeId = endNodeId;
			CrossSectionId = crossSectionId;
		}

		public int Id { get; set; }
		public int BeginNodeId { get; set; }
		public int EndNodeId { get; set; }
		public int CrossSectionId { get; set; }
	}
}
