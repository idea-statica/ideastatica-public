using System;

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
