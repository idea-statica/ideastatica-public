namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConConnection
	{
		//Need to choose to either use Id or Identifier for connection
		public int Id { get; set; }

		public string Identifier { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		//Related to connection?
		public ConAnalysisTypeEnum AnalysisType { get; set; }

		public int BearingMemberId { get; set; }

		public bool IsCalculated { get; }
	}
}
