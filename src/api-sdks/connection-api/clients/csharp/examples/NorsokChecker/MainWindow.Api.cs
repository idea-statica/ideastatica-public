using System.IO;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;
using NorsokChecker.Services;

namespace NorsokChecker
{
	/// <summary>
	/// Everything between the app and the Connection REST API: finding an installed service,
	/// spawning or attaching to one, opening the project, and reading its members, cross-sections
	/// and load effects.
	///
	/// Separated because it changes for its own reasons — a new service version, a changed
	/// endpoint — and none of those are reasons for the check or the tabs to change.
	/// </summary>
	public partial class MainWindow
	{
		internal static string? ServiceRootForTest;

		internal void PrefillServicePath(string? rootOverride = null)
		{
			// A URL in the box means we are coming back from attach mode, and it must not stay in a
			// box that now means a folder. Falls back to the conventional root, which ResolveSetupDir
			// then reports properly if it holds no exe.
			const string fallback = ServiceLocator.DefaultRoot + @"\StatiCa 26.0";
			bool boxHoldsUrl = TxtApiPath.Text.TrimStart()
				.StartsWith("http", StringComparison.OrdinalIgnoreCase);

			try
			{
				var installs = ServiceLocator.FindInstalls(rootOverride)
					.Where(i => ServiceLocator.IsUsableVersion(i.Version))
					.ToList();
				if (installs.Count == 0)
				{
					if (boxHoldsUrl) TxtApiPath.Text = fallback;
					return;
				}

				TxtApiPath.Text = installs[0].Directory;
				Log($"Service: {installs[0].Directory} (v{installs[0].Label})"
					+ (installs.Count > 1
						? $" — also installed: {string.Join(", ", installs.Skip(1).Select(i => i.Label))}"
						: ""));
			}
			catch (Exception ex)
			{
				// never block start-up over this
				if (boxHoldsUrl) TxtApiPath.Text = fallback;
				Log($"Could not detect the installed service ({ex.Message}) — using the default path.");
			}
		}

		private async Task<IConnectionApiClient> CreateApiClientAsync()
		{
			if (RbAttach.IsChecked == true)
			{
				var url = TxtApiPath.Text.Trim();
				string? version = await ServiceLocator.RunningVersionAsync(url);
				if (version == null)
					throw new InvalidOperationException(
						$"No Connection REST API is answering at {url}.\n\n"
						+ "Either start one there, or switch to \"Spawn local API\" to have this app "
						+ "start its own.");

				// A hard stop, not a warning. A service outside the supported range answers every
				// call, so the only symptom is that the IOM export comes back empty — and §6.4 then
				// reports "feet overlap" for a joint whose feet are clear, which is a false
				// statement about the user's model rather than a visible failure. Refusing to
				// connect is the only way the user finds out at all. See ServiceLocator.MaxVersion.
				if (!ServiceLocator.IsSupported(version))
					throw new InvalidOperationException(
						$"The service at {url} reports version {version}, which this app cannot use.\n\n"
						+ $"Supported: {ServiceLocator.MinVersion.Major}.{ServiceLocator.MinVersion.Minor}"
						+ $" to {ServiceLocator.MaxVersion.Major}.{ServiceLocator.MaxVersion.Minor}.x.\n\n"
						+ (ServiceLocator.VersionOf(version) is { } v && v > ServiceLocator.MaxVersion
							? "This one is NEWER than the API client this app is built against. It "
							+ "would answer every call, but the IOM export would come back empty, and "
							+ "§6.4 would then reject sound joints for overlapping feet — a wrong "
							+ "answer rather than an obvious failure, which is why this is refused "
							+ "rather than warned about."
							: "The /api/4 endpoints this app uses do not exist in versions before "
							+ $"{ServiceLocator.MinVersion.Major}.{ServiceLocator.MinVersion.Minor}.")
						+ "\n\nSwitch to \"Spawn local API\" to have this app start a supported one.");

				Log($"Attached to the service at {url} (v{version})");
				return await new ConnectionApiServiceAttacher(url).CreateApiClient();
			}

			// ── spawn: start our own, and do NOT go looking for a running one ──
			// The two radio buttons mean two different things, and mixing them was a defect: with
			// "Spawn local API" chosen the user asked for their own service, so probing port 5000
			// belongs to "Attach to running service" and nowhere else. Reusing whatever happened to
			// be listening also picked up a service NEWER than this app's API client, and the IOM
			// export then deserialises to null — D/T unreadable, every gap negative, §6.4 rejecting
			// a sound joint for "feet overlap". See ServiceLocator.MaxVersion.
			var setupDir = ResolveSetupDir(TxtApiPath.Text.Trim());

			// noted BEFORE the launch: the process is identified by having started after this moment,
			// so a service the user already had running can never be mistaken for ours
			var launchedAfter = DateTime.Now.AddSeconds(-1);

			_runner ??= new ConnectionApiServiceRunner(setupDir);
			var client = await _runner.CreateApiClient();
			_startedOwnService = true;

			// Adopt it into a Job Object so Windows takes it down with us even on a hard kill —
			// otherwise a killed app leaves the service holding a licence seat. The SDK runner keeps
			// its Process private, hence the search.
			_reaper ??= new ServiceReaper(Log);
			var proc = ServiceReaper.FindServiceStartedAfter(launchedAfter);
			if (proc == null)
				Log("  note: the service process could not be identified, so only an orderly close "
					+ "will shut it down");
			else if (_reaper.Adopt(proc))
				Log($"Started our own service (pid {proc.Id}) — it is shut down when this app closes, "
					+ "even if this app is killed.");
			else
				Log($"Started our own service (pid {proc.Id}) — it is shut down when this app closes "
					+ "normally.");

			return client;
		}

		/// <summary>
		/// The folder to launch the service from: what the user typed, if it holds the exe;
		/// otherwise the best installed version, so a stale or empty path still works.
		/// </summary>
		private string ResolveSetupDir(string typed)
		{
			if (!string.IsNullOrWhiteSpace(typed)
				&& File.Exists(Path.Combine(typed, ServiceLocator.ExeName)))
			{
				// A typed path outside the supported range is refused, not warned about. It used to
				// log and start anyway ("the first call will say") — but on a NEWER service nothing
				// fails: every call is answered and only the IOM comes back empty, so §6.4 quietly
				// reports overlapping feet for a sound joint. And starting it takes a licence seat
				// for a service that cannot be used.
				var v = ServiceLocator.VersionOfFolder(typed);
				if (v.Major > 0 && !ServiceLocator.IsUsableVersion(v))
					throw new InvalidOperationException(
						$"{typed} looks like version {v.Major}.{v.Minor}, which this app cannot use.\n\n"
						+ $"Supported: {ServiceLocator.MinVersion.Major}.{ServiceLocator.MinVersion.Minor}"
						+ $" to {ServiceLocator.MaxVersion.Major}.{ServiceLocator.MaxVersion.Minor}.x.\n\n"
						+ (v > ServiceLocator.MaxVersion
							? "A newer service answers every call, but this app's API client cannot "
							+ "read the model from it — §6.4 would then reject sound joints for "
							+ "overlapping feet instead of failing visibly."
							: "The /api/4 endpoints this app uses do not exist in earlier versions.")
						+ "\n\nClear the box to let the app pick an installed version itself.");
				return typed;
			}

			var installs = ServiceLocator.FindInstalls();
			// IsUsableVersion, not just MinVersion: a 26.1 install is present on developer
			// machines and would otherwise be picked, and this app's API client cannot read the IOM
			// from it (see ServiceLocator.MaxVersion).
			var usable = installs.Where(i => ServiceLocator.IsUsableVersion(i.Version)).ToList();

			if (usable.Count == 0)
			{
				string detail = installs.Count == 0
					? $"No {ServiceLocator.ExeName} was found under {ServiceLocator.DefaultRoot}"
					  + (ServiceLocator.RegistryInstallDir() is { } r ? $" or {r}" : "") + "."
					// naming what IS installed matters: "nothing found" sends the user looking for a
					// missing install when the real problem is the version. And the range has TWO
					// ends — on a machine with only 26.1 the answer is "too new", and saying "too
					// old" there would send them to install something even newer.
					: "None of the installed versions can be used by this app: "
					  + string.Join(", ", installs.Select(i => i.Label))
					  + $". Supported: {ServiceLocator.MinVersion.Major}.{ServiceLocator.MinVersion.Minor}"
					  + $" to {ServiceLocator.MaxVersion.Major}.{ServiceLocator.MaxVersion.Minor}.x"
					  + " — /api/4 does not exist before that, and this app's API client cannot read"
					  + " the model from a newer service.";
				throw new InvalidOperationException(
					$"Cannot start the Connection REST API.\n\n{detail}\n\n"
					+ $"You can also set {ServiceLocator.OverrideVariable} to the exe, or point the "
					+ "folder box at an installation yourself.");
			}

			var best = usable[0];
			Log($"Service folder: {best.Directory} (v{best.Label})"
				+ (usable.Count > 1
					? " — also installed: " + string.Join(", ", usable.Skip(1).Select(i => i.Label))
					: ""));
			return best.Directory;
		}

		private async Task LoadLoadEffectCountsAsync()
		{
			if (_apiClient == null || _projectId == Guid.Empty) return;

			foreach (var con in _connections)
			{
				try
				{
					// isPercentage is irrelevant here — only Active and the count are read — but the
					// flag is passed explicitly so this call cannot be mistaken for one that reads forces.
					var les = await _apiClient.LoadEffect.GetLoadEffectsAsync(_projectId, con.Id, isPercentage: false);
					con.TotalLoadEffects = les.Count;
					con.ActiveLoadEffects = les.Count(le => le.Active);
				}
				catch (Exception ex)
				{
					Log($"  WARNING: could not read load effects of {con.Name}: {ex.Message}");
				}
			}

			int known = _connections.Count(c => c.TotalLoadEffects >= 0);
			if (known > 0)
				Log($"  load effects: {_connections.Where(c => c.ActiveLoadEffects >= 0).Sum(c => c.ActiveLoadEffects)}"
					+ $" active of {_connections.Where(c => c.TotalLoadEffects >= 0).Sum(c => c.TotalLoadEffects)}"
					+ $" across {known} connection(s)");
		}

		/// <summary>
		/// Read the members of EVERY connection once, at load time, and cache them. Switching
		/// connections then only swaps the grid contents — no API calls, no log noise, no waiting.
		/// </summary>
		private async Task LoadAllConnectionMembersAsync()
		{
			if (_apiClient == null || _projectId == Guid.Empty) return;

			_membersPerConnection.Clear();

			// project-wide, so it is fetched once rather than per connection
			var detectedCss = await new CrossSectionDetector(_apiClient, Log).DetectAsync(_projectId);

			foreach (var con in _connections)
			{
				ShowStatus($"Reading members of {con.Name}...");
				try
				{
					_membersPerConnection[con.Id] = await ReadMembersAsync(con, detectedCss);
					Log($"  {con.Name}: {_membersPerConnection[con.Id].Count} member(s)");
				}
				catch (Exception ex)
				{
					_membersPerConnection[con.Id] = new List<MemberDisplayInfo>();
					Log($"  WARNING: could not read members of {con.Name}: {ex.Message}");
				}
			}
		}

		/// <summary>Show a cached connection's members. No API traffic.</summary>

		private async Task<List<MemberDisplayInfo>> ReadMembersAsync(
			ConnectionCheckResult con, List<DetectedCrossSection> detectedCss)
		{
			var result = new List<MemberDisplayInfo>();
			var geoReader = new MemberGeometryReader(_apiClient!, Log);
			var memberInfos = await geoReader.ReadMembersAsync(
				_projectId, con.Id, rawResults: null, ct: default);

			foreach (var info in memberInfos)
			{
				double diameter = 0;
				double wallThickness = info.WallThickness;
				string shape = info.ShapeType;

				// The member's OWN cross-section, matched by id. This used to sort the project's
				// sections by diameter and take the largest for a continuous member and the
				// smallest for every other — so on a joint with several profiles every member
				// but one got the wrong section. Measured on test_cs CON1: four different braces
				// all reported PIPE127STD, the chord got the smallest section, and D/t came out
				// 0 wherever that name did not parse.
				var matchCss = detectedCss.FirstOrDefault(c => c.Id == info.CrossSectionId);
				if (matchCss != null)
				{
					shape = matchCss.ShapeType;
					if (matchCss.Diameter > 0) diameter = matchCss.Diameter;
					if (matchCss.Thickness > 0) wallThickness = matchCss.Thickness;
				}
				else if (info.CrossSectionId != null)
				{
					Log($"  WARNING: member '{info.Name}' references cross-section "
						+ $"{info.CrossSectionId}, which was not read — D/t unknown");
				}

				result.Add(new MemberDisplayInfo
				{
					Id = info.Id,
					Name = info.Name,
					Role = info.IsContinuous ? "Chord" : "Brace",
					Shape = shape,
					Profile = matchCss?.Name ?? "",
					Diameter = diameter,
					WallThickness = wallThickness,
					// material and fy come from the cross-section, so they are known before any
					// calculation; the raw-results values (when a run happens) refine them
					Fy = info.Fy > 0 ? info.Fy : matchCss?.Fy > 0 ? matchCss.Fy : 355,
					MaterialName = !string.IsNullOrEmpty(info.MaterialName)
						? info.MaterialName
						: matchCss?.MaterialName ?? "",
				});
			}

			// D/T from the connection's own model wherever it can be read — the section name is
			// wrong for most catalogue circular profiles. See TubeFromIom.
			await EnrichFromIomAsync(con.Id, result);
			return result;
		}

		/// <summary>
		/// Overwrite the grid's D/t with the values measured from the IOM facet ring, matched by
		/// member name. Same source as the §6.4 path uses — without this the grid would keep showing
		/// the name-parsed values (or 0) while the check ran on different numbers.
		/// </summary>
		private async Task EnrichFromIomAsync(int connectionId, List<MemberDisplayInfo> grid)
		{
			IdeaRS.OpenModel.Connection.ConnectionData? iom;
			try
			{
				iom = await _apiClient!.Export.ExportIomConnectionDataAsync(_projectId, connectionId);
			}
			catch (Exception ex)
			{
				Log($"  IOM export failed ({ex.Message}) — D/t stay as read from the cross-sections");
				return;
			}

			var beams = Services.Norsok64.TubeFromIom.TubularBeamsByName(iom);
			foreach (var m in grid)
			{
				if (!beams.TryGetValue(m.Name, out var beam)) continue;
				var (d, t, why) = Services.Norsok64.TubeFromIom.FromBeam(beam);
				if (d is not > 0 || t is not > 0)
				{
					Log($"  IOM: '{m.Name}' D/t not readable ({why})");
					continue;
				}
				bool changed = Math.Abs(m.Diameter - d.Value) > 0.05 || Math.Abs(m.WallThickness - t.Value) > 0.01;
				if (changed)
					Log($"  IOM: '{m.Name}' Ø{d:F1}/{t:F1} mm from the model "
						+ $"(cross-section said Ø{m.Diameter:F1}/{m.WallThickness:F1})");
				m.Diameter = d.Value;
				m.WallThickness = t.Value;
				m.Shape = "CHS";
			}
		}

		/// <summary>
		/// The API configuration and the log belong to setting the run up, so they are shown on the
		/// Check tab only. They used to sit outside the tab control and take vertical space on
		/// Results and Report, where neither is any use.
		/// </summary>

		private async Task EnrichSectionsFromIomAsync(
			int connectionId, List<Services.Norsok64.JointMemberData> members)
		{
			IdeaRS.OpenModel.Connection.ConnectionData? iom;
			try
			{
				iom = await _apiClient!.Export.ExportIomConnectionDataAsync(_projectId, connectionId);
			}
			catch (Exception ex)
			{
				Log($"    WARNING: IOM export failed ({ex.Message}) — D/T stay as parsed from the section names");
				return;
			}

			// An export that comes back empty is NOT the same as a model without tubes, and saying
			// "no tubular beams in the model" for it is a false statement about the user's model.
			//
			// Measured 2026-08-27 against service 26.1.0.2007, and the cause is the CLIENT, not the
			// service: on one open project, in one session, the endpoint returns HTTP 200 with
			// 418 389 characters and all six beams over raw HTTP, while
			// ExportIomConnectionDataAsync deserialises that same response to null. The two payloads
			// (26.0 and 26.1) are structurally identical — same top-level keys, same beam keys, same
			// plate keys — so what breaks is inside the generated client. Against 26.0 the same
			// client returns all six beams. See IomExportVersionTests, which pins the distinction.
			//
			// An earlier version of this comment blamed the service. It was wrong, and the direction
			// matters: a broken service means waiting for a service build, a broken client means an
			// upgrade or reading the payload ourselves.
			//
			// Either way D/T then come from nowhere, and because the gaps are computed from the
			// diameters, every gap goes negative and §6.4 rejects the joint for "feet overlap" —
			// a conclusion about geometry drawn from the absence of geometry. Say plainly that the
			// model could not be read.
			if (iom == null || iom.Beams == null || iom.Beams.Count == 0)
			{
				Log("    WARNING: the IOM export returned no model for this connection"
					+ $" ({(iom == null ? "no data at all" : "no beams")})"
					+ " — D/T cannot be read from the model, so any tube whose section name does not"
					+ " spell out its dimensions will be reported as unreadable");
				return;
			}

			var beams = Services.Norsok64.TubeFromIom.TubularBeamsByName(iom);
			if (beams.Count == 0)
			{
				Log($"    IOM: the model has {iom.Beams.Count} beam(s) but none of a tubular type"
					+ " — D/T stay as parsed from the section names");
				return;
			}

			foreach (var m in members)
			{
				// only tubular members: the facet formula would return a plausible-looking number
				// for an I-section too, and that is worse than no number at all
				if (!beams.TryGetValue(m.Name ?? "", out var beam)) continue;

				var (d, t, why) = Services.Norsok64.TubeFromIom.FromBeam(beam);
				if (d is not > 0 || t is not > 0)
				{
					Log($"    IOM: '{m.Name}' D/T not readable ({why}) — keeping the name-derived values");
					continue;
				}

				double? nameD = m.Section.D, nameT = m.Section.T;
				m.Section.D = d;
				m.Section.T = t;
				m.Section.IsCHS = true;

				// Cross-check the name against the model. This is the whole reason for reading the
				// facet ring: "PIPE127STD" is really Ø141.3, because 127 is the nominal size. Over
				// 2 % apart, the disagreement is recorded so the report can say so rather than
				// silently using a different number than the name implies.
				if (nameD is > 0 && Math.Abs(nameD.Value - d.Value) / d.Value > 0.02)
				{
					m.Section.GeomNote = $"the section name suggests D = {nameD:F1} mm but the model "
						+ $"has D = {d:F1} mm — using the model";
					Log($"    IOM: '{m.Name}' {m.Section.GeomNote}");
				}

				string cross = nameD is > 0 && nameT is > 0
					? $" (name said Ø{nameD:F1}/{nameT:F1})"
					: " (name gave nothing)";
				Log($"    IOM: '{m.Name}' Ø{d:F1}/{t:F1} mm from {beam.Plates.Count} facets{cross}");
			}
		}



		/// <summary>
		/// Fill the results grids. Results holds every check; the per-chapter tabs hold the same rows
		/// grouped, so §6.4 detail is not buried among the plate and weld checks.
		///
		/// The rows are ordered so that a joint's conditions and assumptions come before its checks —
		/// reading "outside the scope" after a table of utilisations is the wrong way round.
		/// </summary>

	}
}
