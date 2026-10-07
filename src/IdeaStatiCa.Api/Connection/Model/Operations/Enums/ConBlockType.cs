namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Whether the operation creates a new concrete block, anchors into an existing one, or has no block.
	/// </summary>
	public enum ConBlockType
	{
		/// <summary>Operation creates a new foundation block.</summary>
		New,

		/// <summary>Operation anchors into a foundation block produced by another operation.</summary>
		Existing,

		/// <summary>No concrete block — a plate-to-plate contact. Valid for a contact grid only.</summary>
		No,
	}
}
