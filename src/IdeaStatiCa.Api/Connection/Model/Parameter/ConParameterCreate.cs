// This project does not enable nullable reference types, and these contracts rely on the
// annotations to say which fields are omitted rather than empty.
#nullable enable

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Parameters
{
	/// <summary>
	/// Definition of a new parameter. Field names match <c>IdeaParameter</c> so create, read and update
	/// speak the same language.
	/// </summary>
	public class ConParameterCreate
	{
		/// <summary>Identifier used in expressions and when linking; unique within the connection.</summary>
		public string Key { get; set; } = string.Empty;

		/// <summary>Int, Float, Bool, String or Expression. Library-typed parameters are not supported yet.</summary>
		public string ParameterType { get; set; } = string.Empty;

		/// <summary>The value, or an expression evaluating to it.</summary>
		public string Expression { get; set; } = string.Empty;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Description { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? LowerBound { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? UpperBound { get; set; }
	}

	/// <summary>
	/// Names the property a parameter is being linked to — the same shape the catalog returns as
	/// <c>owner</c> plus the property id, so a catalog row round-trips into a link request.
	/// </summary>
	public class ConParameterLinkCreate
	{
		/// <summary>Identifier of the parameter that will drive the property.</summary>
		public string Parameter { get; set; } = string.Empty;

		public ConPropertyOwnerKind Kind { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? OperationId { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MemberId { get; set; }

		/// <summary>Opaque property identifier taken from the catalog; never assembled by the caller.</summary>
		public string PropertyId { get; set; } = string.Empty;
	}

	/// <summary>An established parameter-to-property link.</summary>
	public class ConParameterLink
	{
		public int Id { get; set; }

		public string Parameter { get; set; } = string.Empty;

		public ConPropertyOwner Owner { get; set; } = new ConPropertyOwner();

		public string PropertyId { get; set; } = string.Empty;
	}

	/// <summary>Outcome of deleting a parameter.</summary>
	public class ConParameterDeleteResult
	{
		public string Key { get; set; } = string.Empty;

		/// <summary>How many parameter-to-property links were deleted along with the parameter.</summary>
		public int DeletedLinks { get; set; }
	}
}
