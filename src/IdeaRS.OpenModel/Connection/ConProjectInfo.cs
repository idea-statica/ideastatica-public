using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Provides informationabout
	/// </summary>
	[DataContract]
	public class ConProjectInfo
	{
		/// <summary>
		/// The name of the project
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// The description of the project
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Project number
		/// </summary>
		[DataMember]
		public string ProjectNumber { get; set; }

		/// <summary>
		/// Name of the author
		/// </summary>
		[DataMember]
		public string Author { get; set; }

		/// <summary>
		/// Design code
		/// </summary>
		[DataMember]
		public string DesignCode { get; set; }

		/// <summary>
		/// Source Application specify data source
		/// </summary>
		[DataMember]
		public string SourceApplication { get; set; }

		/// <summary>
		/// Date
		/// </summary>
		[DataMember]
		public DateTime Date { get; set; }

		/// <summary>
		/// Connections in the project
		/// </summary>
		[DataMember]
		public List<ConnectionInfo> Connections { get; set; }
	}
}