using System;

namespace IdeaStatiCa.Plugin.Exeptions
{
	/// <summary>
	/// Exception thrown when a bulk selection process exceeds the allowed limit, 
	/// resulting in all members being grouped into a single connection.
	/// </summary>
	public class BulkSelectionOverflowException : Exception
	{
		/// <summary>
		/// Gets the number of members in the connection that caused the exception.
		/// </summary>
		public int? MembersInConnection { get; }

		/// <summary>
		/// Gets the theoretical size limit for a valid connection.
		/// </summary>
		public int? ConnectionSizeLimit { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BulkSelectionOverflowException"/> class
		/// with a default error message.
		/// </summary>
		public BulkSelectionOverflowException()
			: base("Bulk selection exceeded the allowed limit and combined all members into one connection.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BulkSelectionOverflowException"/> class
		/// with a specified error message.
		/// </summary>
		/// <param name="message">The error message describing the cause of the exception.</param>
		public BulkSelectionOverflowException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BulkSelectionOverflowException"/> class
		/// with a specified error message and an inner exception that caused this exception.
		/// </summary>
		/// <param name="message">The error message describing the cause of the exception.</param>
		/// <param name="innerException">The exception that caused the current exception.</param>
		public BulkSelectionOverflowException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BulkSelectionOverflowException"/> class
		/// including the number of members in the connection.
		/// </summary>
		/// <param name="membersInConnection">The number of members in the connection.</param>
		public BulkSelectionOverflowException(int membersInConnection)
			: base($"Bulk selection included {membersInConnection} members, exceeding the allowed limit.")
		{
			MembersInConnection = membersInConnection;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BulkSelectionOverflowException"/> class
		/// including the number of members in the connection and the theoretical size limit.
		/// </summary>
		/// <param name="membersInConnection">The number of members in the connection.</param>
		/// <param name="connectionSizeLimit">The theoretical size limit for a valid connection.</param>
		public BulkSelectionOverflowException(int membersInConnection, int connectionSizeLimit)
			: base($"Bulk selection included {membersInConnection} members, exceeding the theoretical connection size limit of {connectionSizeLimit}.")
		{
			MembersInConnection = membersInConnection;
			ConnectionSizeLimit = connectionSizeLimit;
		}
	}
}
