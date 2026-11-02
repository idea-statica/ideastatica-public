namespace IdeaRS.Connections.Model
{
	public interface IConnectionPart
	{
		/// <summary>
		/// If true  the connection part is valid for the analysis model
		/// </summary>
		bool IsValidForAnalysis { get; set; }

		int OperationId { get; }

		int OperationSubId { get; }

		/// <summary>
		/// Id which is unique within whole connection
		/// </summary>
		int UniqueId { get; set; }

		/// <summary>
		/// Id which is unique within compound connection part
		/// </summary>
		int Id { get; set; }

		/// <summary>
		/// Gets or sets properties of the part (created by user)
		/// </summary>
		object Parameters { get; set; }

		/// <summary>
		/// Gets or sets the name of the connection part
		/// </summary>
		string Name { get; set; }
	}
}