#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Contact grid operation. A base plate placed against a foundation block (no fasteners).
	/// Maps to the new-model <c>ContactGridOperation</c>.
	///
	/// <para>Not to be confused with the "Contact" element of the WeldOrContact operation,
	/// which is a different concept (plate-to-plate contact within a steel connection).</para>
	/// </summary>
	public class ConContactGridOperation : ConOperation
	{
		public ConContactGridOperation() : base()
		{
			Active = true;
		}

		[JsonConstructor]
		public ConContactGridOperation(int id) : base(id)
		{
			Active = true;
		}

		/// <summary>Plates / members connected by the contact.</summary>
		public List<ConConnectedItem> ConnectedItems { get; set; } = new List<ConConnectedItem>();

		/// <summary>New block properties. Required when <see cref="BlockType"/> is <see cref="ConBlockType.New"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConFoundationBlockDto? FoundationBlock { get; set; }

		public ConBlockType BlockType { get; set; } = ConBlockType.New;

		/// <summary>Operation ID of the existing block. Required when <see cref="BlockType"/> is <see cref="ConBlockType.Existing"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? ExistingBlockOperationId { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConPlateSide? PlateSide { get; set; }
	}
}
