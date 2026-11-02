// This project does not enable nullable reference types, and these contracts rely on the
// annotations to say which fields are omitted rather than empty.
#nullable enable

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Parameters
{
	/// <summary>
	/// The kind of object a linkable property belongs to. Mirrors the roots the parameter catalog
	/// enumerates: operations, members and the library items a connection edits in place.
	/// </summary>
	public enum ConPropertyOwnerKind
	{
		Operation,
		Member,
		CrossSection,
		Material,
		BoltAssembly,
	}

	/// <summary>
	/// Names the object a property belongs to. Populated only by the connection-wide catalog — on the
	/// per-object routes the owner is already in the URL. The same shape is accepted when creating a
	/// link, so a catalog row round-trips into a link request.
	/// </summary>
	public class ConPropertyOwner
	{
		public ConPropertyOwnerKind Kind { get; set; }

		/// <summary>Required when <see cref="Kind"/> is <see cref="ConPropertyOwnerKind.Operation"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? OperationId { get; set; }

		/// <summary>Required when <see cref="Kind"/> is <see cref="ConPropertyOwnerKind.Member"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MemberId { get; set; }

		/// <summary>Required when <see cref="Kind"/> is <see cref="ConPropertyOwnerKind.CrossSection"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? CrossSectionId { get; set; }

		/// <summary>Required when <see cref="Kind"/> is <see cref="ConPropertyOwnerKind.Material"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MaterialId { get; set; }

		/// <summary>Required when <see cref="Kind"/> is <see cref="ConPropertyOwnerKind.BoltAssembly"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? BoltAssemblyId { get; set; }

		/// <summary>Display name of the object, e.g. <c>SEP1</c>, <c>B2</c>, <c>HEA260</c>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Name { get; set; }
	}

	/// <summary>
	/// A model property a parameter can be linked to.
	/// </summary>
	public class ConLinkableProperty
	{
		/// <summary>
		/// Opaque, stable identifier of the property within its owner — copy it into the link request,
		/// never build it. Usually the numeric define id (<c>"14"</c>), but also <c>"IsActive"</c> and
		/// weld sub-properties such as <c>"31;Size"</c>.
		/// </summary>
		public string PropertyId { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		/// <summary>Property group within the owner, e.g. <c>Plate</c>. Omitted where the owner has no groups (members).</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Group { get; set; }

		/// <summary>Short explanation of what the property drives; complements a generic <see cref="Name"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Description { get; set; }

		/// <summary>Parameter type a linked parameter must have: Int, Float, Bool, String, Css, Material, Bolt, ...</summary>
		public string ValueType { get; set; } = string.Empty;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Unit { get; set; }

		/// <summary>
		/// What the property holds right now, in model units. Omitted where the value is not read —
		/// non-scalar properties, members and library items.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public object? CurrentValue { get; set; }

		/// <summary>Identifier of the parameter currently driving this property; omitted when unlinked.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? LinkedParameter { get; set; }

		/// <summary>Set only by the connection-wide catalog; omitted on the per-object routes.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConPropertyOwner? Owner { get; set; }
	}
}
