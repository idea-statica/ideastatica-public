using System.Text.Json;
using System.Text.Json.Nodes;

namespace NorsokChecker.Services
{
	/// <summary>
	/// What a drawn body is. The presentation payload's own `type.kind` values, mapped onto the ones
	/// the app draws differently; anything unrecognised arrives as <see cref="Other"/> and is still
	/// drawn, so a payload that grows a new kind shows up rather than silently disappearing.
	/// </summary>
	public enum BodyKind
	{
		Member,
		Plate,
		Weld,
		BoltGrid,
		AnchorGrid,
		Other,
	}

	/// <summary>One drawn body, ready for a WPF MeshGeometry3D.</summary>
	public sealed class MemberMesh
	{
		/// <summary>
		/// The member id for a <see cref="BodyKind.Member"/>, and -1 for everything else.
		///
		/// Plates, welds and bolt grids carry their own ids in the same field of the payload, which
		/// do NOT index the connection's members — treating them as member ids would make a plate
		/// highlight a member when a table row is hovered.
		/// </summary>
		public int MemberId { get; set; } = -1;

		public BodyKind Kind { get; set; } = BodyKind.Member;

		public string Name { get; set; } = "";
		/// <summary>Vertex coordinates in metres, flattened x,y,z — the node is at (0,0,0).</summary>
		public double[] Positions { get; set; } = Array.Empty<double>();
		public double[] Normals { get; set; } = Array.Empty<double>();
		/// <summary>Triangle indices into <see cref="Positions"/> (as points, not floats).</summary>
		public int[] Indices { get; set; } = Array.Empty<int>();
	}

	/// <summary>
	/// Reads the drawn geometry of a connection from the API's own presentation payload, so the app
	/// does not have to build a joint picture of its own.
	///
	/// `GET .../connections/{id}/presentations/text` returns one group per drawn object, each tagged
	/// `selected: { id, type: { kind } }`. Vertices and normals are shared arrays; each group's
	/// `faces` are indices into them.
	///
	/// EVERY kind is returned, not just members. Measured on test_cs (2026-08-31): the kinds present
	/// are member, weld, plate and boltGrid, and keeping members only dropped 27–30 % of the geometry
	/// on the plain tubular joints and 83 % on CON15 — which is a plated, bolted joint, so what the
	/// filter removed was most of what there is to see. The reason it was written that way (welds
	/// tripling the triangle count for something §6.4 never refers to) is a rendering-cost argument,
	/// and it was traded for a picture that does not match the model.
	/// </summary>
	public static class JointPresentationReader
	{
		private static BodyKind KindOf(string kind) => kind.ToLowerInvariant() switch
		{
			"member" => BodyKind.Member,
			"plate" => BodyKind.Plate,
			"weld" => BodyKind.Weld,
			"boltgrid" => BodyKind.BoltGrid,
			"anchorgrid" => BodyKind.AnchorGrid,
			_ => BodyKind.Other,
		};

		/// <param name="knownMemberIds">
		/// The connection's real member ids, from GET .../members. When given, a body tagged "member"
		/// whose id is NOT in this set is drawn as context instead of as an assessable member.
		///
		/// This is needed because the payload's "member" kind is broader than the connection's member
		/// list. Measured on test_cs CON15 (2026-08-31): nine groups come back tagged "member" while
		/// /members reports six — ids 7, 8 and 9 are extra bodies with no member entry and no label.
		/// Trusting the tag alone would put three unassessable bodies in the members collection, so
		/// the table would say nine members with three of them permanently without a check.
		///
		/// Omit it and every "member" tag is taken at face value, which is right for a payload read on
		/// its own (as the unit tests do).
		/// </param>
		public static List<MemberMesh> ReadMembers(string presentationJson, Action<string>? log = null,
			IReadOnlyCollection<int>? knownMemberIds = null)
		{
			var result = new List<MemberMesh>();

			JsonNode? root;
			try
			{
				root = JsonNode.Parse(presentationJson);
				// the payload is occasionally delivered as a JSON string containing JSON
				if (root is JsonValue v && v.TryGetValue<string>(out var inner))
					root = JsonNode.Parse(inner);
			}
			catch (JsonException ex)
			{
				log?.Invoke($"  3D: presentation payload could not be parsed ({ex.Message})");
				return result;
			}

			var verts = root?["vertices"]?.AsArray();
			var norms = root?["normals"]?.AsArray();
			var groups = root?["groups"]?.AsArray();
			if (verts == null || groups == null)
			{
				log?.Invoke("  3D: presentation payload carries no vertices or groups");
				return result;
			}

			// shared arrays, read once
			var vAll = new double[verts.Count];
			for (int i = 0; i < verts.Count; i++) vAll[i] = verts[i]?.GetValue<double>() ?? 0.0;
			var nAll = new double[norms?.Count ?? 0];
			for (int i = 0; i < nAll.Length; i++) nAll[i] = norms![i]?.GetValue<double>() ?? 0.0;

			foreach (var g in groups)
			{
				var sel = g?["selected"];
				string kindText = sel?["type"]?["kind"]?.GetValue<string>() ?? "";
				var kind = KindOf(kindText);

				// A member id ONLY for a member. The checks are per member — every §6.4 result, the
				// utilisation colouring and the table's hover selection are keyed on this id — while
				// plates, welds and bolts are context: drawn, never assessed. Giving them the id
				// found in the same field would let a plate answer to a member's row.
				int memberId = -1;
				if (kind == BodyKind.Member)
				{
					string idStr = sel?["id"]?.GetValue<string>() ?? "";
					if (!int.TryParse(idStr, out memberId)) continue;

					// tagged "member" but not one of the connection's members — an extra body. Drawn,
					// but demoted so nothing tries to check it (see knownMemberIds).
					if (knownMemberIds != null && !knownMemberIds.Contains(memberId))
					{
						kind = BodyKind.Other;
						memberId = -1;
					}
				}

				var faces = g?["faces"]?.AsArray();
				if (faces == null || faces.Count < 3) continue;

				var idx = new int[faces.Count];
				for (int i = 0; i < faces.Count; i++) idx[i] = faces[i]?.GetValue<int>() ?? 0;

				result.Add(new MemberMesh
				{
					MemberId = memberId,
					Kind = kind,
					Name = g?["text"]?.AsArray()?.FirstOrDefault()?["value"]?.GetValue<string>() ?? "",
					Positions = vAll,
					Normals = nAll,
					Indices = idx,
				});
			}

			int members = result.Count(m => m.Kind == BodyKind.Member);
			string breakdown = string.Join(", ", result
				.GroupBy(m => m.Kind)
				.OrderByDescending(gr => gr.Count())
				.Select(gr => $"{gr.Count()} {gr.Key.ToString().ToLowerInvariant()}"));
			log?.Invoke($"  3D: {result.Count} bodies ({breakdown}), {members} assessable, "
				+ $"{vAll.Length / 3} shared points");
			return result;
		}
	}
}
