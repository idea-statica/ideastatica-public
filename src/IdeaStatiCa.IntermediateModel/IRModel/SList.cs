namespace IdeaStatiCa.IntermediateModel.IRModel
{
	// List intermediate object
	public class SList : ISIntermediate
	{
		public ICollection<ISIntermediate> Items { get; set; } = new List<ISIntermediate>();
	}
}
