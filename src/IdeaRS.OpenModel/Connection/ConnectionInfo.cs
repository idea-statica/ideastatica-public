using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Provides information about connection
	/// </summary>
	[DataContract]
	public class ConnectionInfo
	{
		/// <summary>
		/// The name of the connection
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// The description of the connection
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// The id of the connection
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// The identifier of the connection
		/// </summary>
		[DataMember]
		public string Identifier { get; set; }
	}
}