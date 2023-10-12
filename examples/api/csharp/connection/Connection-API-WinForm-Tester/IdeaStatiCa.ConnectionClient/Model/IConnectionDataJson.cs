using System;

namespace IdeaStatiCa.ConnectionClient.Model
{
	public interface IConnectionDataJson
	{
		/// <summary>
		/// Identifier of the connection in a project
		/// </summary>
		Guid ConnectionId { get; }

		/// <summary>
		/// Data in JSON format
		/// </summary>
		string DataJson { get; }
	}
}
