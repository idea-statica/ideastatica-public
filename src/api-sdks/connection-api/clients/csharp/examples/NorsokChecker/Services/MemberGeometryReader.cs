using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Reads member data from the Connection API and extracts cross-section
	/// geometry from raw CBFEM results (plate thicknesses).
	///
	/// Strategy:
	/// - GetMembersAsync → member IDs, names, IsContinuous (chord = continuous)
	/// - Raw results plates → thickness values per member
	/// - Plate names follow convention: "C-bfl 1" = Chord bottom flange,
	///   "B-w 1" = Brace web, etc. "C-" prefix = chord, "B-" prefix = brace
	///
	/// For CHS (tubular) members, the web plate thickness = wall thickness (t),
	/// and the diameter must still be provided by the user or read from parameters.
	/// </summary>
	public class MemberGeometryReader
	{
		private readonly IConnectionApiClient _client;
		private readonly Action<string> _log;

		public MemberGeometryReader(IConnectionApiClient client, Action<string> log)
		{
			_client = client;
			_log = log;
		}

		/// <summary>
		/// Read members for a connection and extract what we can about geometry.
		/// Returns a list of (memberId, name, isContinuous/chord, wallThickness from plates).
		/// </summary>
		public async Task<List<MemberInfo>> ReadMembersAsync(
			Guid projectId, int connectionId,
			ParsedRawResults? rawResults = null,
			CancellationToken ct = default)
		{
			var members = await _client.Member.GetMembersAsync(projectId, connectionId, cancellationToken: ct);
			var result = new List<MemberInfo>();

			_log($"    Members: {members.Count}");

			// Build a map of member prefix → plates from raw results
			// Plate naming: "{MemberName}-{part} {id}" e.g. "C-bfl 1", "C-w 1", "D-arc 1"
			// Part types: bfl/tfl/w = I-section, arc = CHS
			foreach (var m in members)
			{
				var info = new MemberInfo
				{
					Id = m.Id,
					Name = m.Name ?? $"Member {m.Id}",
					IsContinuous = m.IsContinuous,
					CrossSectionId = m.CrossSectionId,
				};

				if (rawResults != null)
				{
					// Match plates by member name prefix: "C-xxx" for member "C"
					string prefix = $"{m.Name}-";
					var memberPlates = rawResults.Plates
						.Where(p => p.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
						.ToList();

					// Also try single-letter prefix patterns
					if (memberPlates.Count == 0 && m.IsContinuous)
					{
						memberPlates = rawResults.Plates
							.Where(p => p.Name.StartsWith("C-", StringComparison.OrdinalIgnoreCase))
							.ToList();
					}
					if (memberPlates.Count == 0 && !m.IsContinuous)
					{
						memberPlates = rawResults.Plates
							.Where(p => p.Name.StartsWith("B-", StringComparison.OrdinalIgnoreCase))
							.ToList();
					}

					if (memberPlates.Count > 0)
					{
						// Detect shape from plate names:
						// "arc" in name → CHS (circular arc segments)
						// "bfl"/"tfl"/"w" → I-section (bottom flange/top flange/web)
						bool hasArc = memberPlates.Any(p => p.Name.Contains("arc", StringComparison.OrdinalIgnoreCase));
						bool hasFlange = memberPlates.Any(p => p.Name.Contains("fl", StringComparison.OrdinalIgnoreCase));
						bool hasWeb = memberPlates.Any(p => p.Name.Contains("-w ", StringComparison.OrdinalIgnoreCase));

						if (hasArc)
							info.ShapeType = "CHS";
						else if (hasFlange && hasWeb)
							info.ShapeType = "I-section";
						else if (hasFlange)
							info.ShapeType = "RHS";
						else
							info.ShapeType = "Other";

						// Wall thickness = most common plate thickness
						var thicknesses = memberPlates
							.Where(p => p.Thickness > 0)
							.Select(p => p.Thickness)
							.ToList();

						if (thicknesses.Count > 0)
						{
							info.WallThickness = thicknesses
								.GroupBy(t => Math.Round(t, 1))
								.OrderByDescending(g => g.Count())
								.First().Key;
						}

						// Material from first plate with f_y
						var refPlate = memberPlates.FirstOrDefault(p => p.MaterialFy > 0);
						if (refPlate != null)
						{
							info.Fy = refPlate.MaterialFy;
							info.MaterialName = refPlate.MaterialName;
						}
					}
				}

				_log($"      [{m.Id}] {info.Name} — {(m.IsContinuous ? "chord" : "brace")}, shape={info.ShapeType}, t={info.WallThickness:F1}mm, fy={info.Fy:F0}MPa ({info.MaterialName})");
				result.Add(info);
			}

			return result;
		}
	}

	public class MemberInfo
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public bool IsContinuous { get; set; }
		public int? CrossSectionId { get; set; }
		/// <summary>"CHS", "I-section", "RHS", "Other"</summary>
		public string ShapeType { get; set; } = "Other";
		public double WallThickness { get; set; }
		public double Fy { get; set; } = 355;
		public string MaterialName { get; set; } = string.Empty;
		public bool IsCHS => ShapeType == "CHS";
	}
}
