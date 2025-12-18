using System;
using System.Text.Json.Serialization;

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

		[Obsolete("This property is currently ignored and not updated")]
		public bool IsCalculated { get; }

		public bool IncludeBuckling { get; set; }
	}
}
