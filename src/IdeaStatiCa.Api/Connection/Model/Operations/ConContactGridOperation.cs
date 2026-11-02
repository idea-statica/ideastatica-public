#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Contact grid operation (no fasteners). A base plate placed against a foundation block, or —
	/// without <see cref="FoundationBlock"/> — plates in contact with each other.
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

		/// <summary>Plates / members connected by the contact. At least two distinct plates when there is no <see cref="FoundationBlock"/>.</summary>
		public List<ConConnectedItem> ConnectedItems { get; set; } = new List<ConConnectedItem>();

		/// <summary>The concrete block the plates bear on — a new one or an existing one — or null for a plate-to-plate contact.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConFoundationBlockDto? FoundationBlock { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConPlateSide? PlateSide { get; set; }
	}
}
