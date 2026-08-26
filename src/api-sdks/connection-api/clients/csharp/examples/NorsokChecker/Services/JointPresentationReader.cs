using System.Text.Json;
using System.Text.Json.Nodes;

namespace NorsokChecker.Services
{
	/// <summary>One member's drawn body, ready for a WPF MeshGeometry3D.</summary>
	public sealed class MemberMesh
	{
		public int MemberId { get; set; }
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
	/// `selected: { id, type: { kind } }` — the id is the member id and the kind is "member" or
	/// "weld". Vertices and normals are shared arrays; each group's `faces` are indices into them.
	/// Verified on test_cs CON1 (6 members + 84 welds, 6 672 points) and CON8.
	///
	/// Only members are returned: welds would triple the triangle count for something the check
	/// never refers to.
	/// </summary>
	public static class JointPresentationReader
	{
		public static List<MemberMesh> ReadMembers(string presentationJson, Action<string>? log = null)
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
				string kind = sel?["type"]?["kind"]?.GetValue<string>() ?? "";
				if (!string.Equals(kind, "member", StringComparison.OrdinalIgnoreCase)) continue;

				// the id arrives as a string
				string idStr = sel?["id"]?.GetValue<string>() ?? "";
				if (!int.TryParse(idStr, out int memberId)) continue;

				var faces = g?["faces"]?.AsArray();
				if (faces == null || faces.Count < 3) continue;

				var idx = new int[faces.Count];
				for (int i = 0; i < faces.Count; i++) idx[i] = faces[i]?.GetValue<int>() ?? 0;

				result.Add(new MemberMesh
				{
					MemberId = memberId,
					Name = g?["text"]?.AsArray()?.FirstOrDefault()?["value"]?.GetValue<string>() ?? "",
					Positions = vAll,
					Normals = nAll,
					Indices = idx,
				});
			}

			log?.Invoke($"  3D: {result.Count} member body/bodies, {vAll.Length / 3} shared points");
			return result;
		}
	}
}
