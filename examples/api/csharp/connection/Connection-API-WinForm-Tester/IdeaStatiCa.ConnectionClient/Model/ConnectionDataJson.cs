using System;

namespace IdeaStatiCa.ConnectionClient.Model
{
	public class ConnectionLoadingJson : ConnectionDataJson
	{
		public ConnectionLoadingJson(Guid connectionId, string json) : base(connectionId, json)
		{
		}
	}

	/// <summary>
	/// Connection data in JSON format
	/// </summary>
	public class ConnectionDataJson : IConnectionDataJson
	{
		string dataJson;
		readonly Guid connectionId;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionId">Id of the connection</param>
		/// <param name="json">JSON string</param>
		public ConnectionDataJson(Guid connectionId, string json)
		{
			this.connectionId = connectionId;
			dataJson = json;
		}

		/// <summary>
		/// Json string represention connection parameters
		/// </summary>
		public string DataJson { get => dataJson; set => dataJson = value; }

		public Guid ConnectionId => connectionId;
	}
}
