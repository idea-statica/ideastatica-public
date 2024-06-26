namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	public class ConMember : ConItem
	{
		public bool IsStiffening { get; set; }

		public bool IsContinuous { get; set; }

		public int CrossSectionId { get; set; }

		public bool MirrorY { get; set; }

		public bool MirrorZ { get; set; }

		public ConMember(int id):base(id)
		{

		}
	}
}