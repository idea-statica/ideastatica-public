using System.ComponentModel;

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

		/// <summary>
		/// True when the connection has calculated results, which the results and report endpoints read.
		/// Set by the service; a value sent in an update is ignored. Changing the analysis type or the
		/// buckling switch removes the results, so it goes back to false.
		/// </summary>
		[ReadOnly(true)]
		public bool IsCalculated { get; set; }

		public bool IncludeBuckling { get; set; }

		/// <summary>
		/// The edition of the steel design code this connection is checked to (for the American code
		/// this is the LRFD/ASD choice).
		///
		/// On update, <see cref="ConSteelCodeEditionEnum.NotSpecified"/> keeps the current edition, so a
		/// client that does not send the field cannot wipe it. An edition that does not belong to the
		/// project's design code is rejected with 422.
		/// </summary>
		public ConSteelCodeEditionEnum SteelEdition { get; set; }
	}
}
