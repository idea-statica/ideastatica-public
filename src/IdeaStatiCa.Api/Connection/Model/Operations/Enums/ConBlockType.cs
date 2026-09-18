namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Whether the operation creates a new concrete block or anchors into an existing one.
	/// </summary>
	public enum ConBlockType
	{
		/// <summary>Operation creates a new foundation block.</summary>
		New,

		/// <summary>Operation anchors into a foundation block produced by another operation.</summary>
		Existing,
	}
}
