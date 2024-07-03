namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	public class ConMember : ConItem
	{
		public bool IsContinuous { get; set; }

		public int CrossSectionId { get; set; }

		public bool MirrorY { get; set; }

		public bool MirrorZ { get; set; }

		public bool IsBearing { get; }

		public ConMember(int id, bool isBearing) : base(id)
		{
			IsBearing = isBearing;
		}
	}
}