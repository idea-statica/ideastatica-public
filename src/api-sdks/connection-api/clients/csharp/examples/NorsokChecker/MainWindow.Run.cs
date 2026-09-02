using System.Windows;
using IdeaStatiCa.Api.Connection.Model;
using NorsokChecker.Models;
using NorsokChecker.Services;

namespace NorsokChecker
{
	/// <summary>
	/// The run: what gets checked, in what order, and what happens to the results.
	///
	/// One method, deliberately in a file of its own — this is the sequence the whole app exists to
	/// perform, and it is the single place a new chapter has to be wired into.
	/// </summary>
	public partial class MainWindow
	{
		private async void RunCheck_Click(object sender, RoutedEventArgs e)
		{
			if (_apiClient == null)
				return;

			_checkCts?.Dispose();
			_checkCts = new CancellationTokenSource();
			var ct = _checkCts.Token;

			// Outside the try because the cancellation handler needs it: on a stop it marks the
			// connections THIS run was working on, and must not touch the ones left unticked.
			var selected = _connections.Where(c => c.Selected).ToList();

			try
			{
				Telemetry.CheckClicked();

				BtnRunCheck.IsEnabled = false;
				BtnCancelCheck.Visibility = Visibility.Visible;
				BtnCancelCheck.IsEnabled = true;
				ValidateGeometryInputs();
				ShowStatus("Running NORSOK N-004 compliance check...");
				Log("Starting Norsok N-004 compliance check...");

				// ── Which chapters? Whatever the registry offers and the user ticked ──
				var selectedChapters = SelectedChapters();
				bool activeLoadEffectsOnly = ChkActiveLoadEffectsOnly.IsChecked == true;
				Log("Chapters: "
					+ (selectedChapters.Count == 0
						? "none selected"
						: string.Join(", ", selectedChapters.Select(c => c.Key)))
					+ $"; load effects: {(activeLoadEffectsOnly ? "active only" : "all in the file")}");

				// §6.4 keeps its topology per connection so its tab can show a single load effect and
				// not just the envelope. Only a chapter with its own tab needs this, which is why it
				// is set here rather than being part of the interface.
				foreach (var c64 in selectedChapters.OfType<Services.Chapters.Chapter64>())
					c64.Topology = (conId, topo) => _topologyPerConnection[conId] = topo;

				// ── Which connections? The per-row checkbox, not the whole project ──
				// Everything downstream works off this list: the engine is asked to calculate only
				// these, only these are evaluated, and only these are reported. A connection left
				// unticked keeps whatever it said before rather than being cleared — the user
				// excluded it from THIS run, which is not the same as having no result.
				if (selected.Count == 0)
				{
					Log("No connection is ticked for assessment — nothing to check.");
					MessageBox.Show("No connection is selected for assessment.\n\n"
						+ "Tick at least one connection in the list.",
						"Nothing to check", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
				if (selected.Count < _connections.Count)
					Log($"Connections: {selected.Count} of {_connections.Count} ticked for assessment "
						+ $"({string.Join(", ", selected.Select(c => c.Name))})");
				else
					Log($"Connections: all {selected.Count} ticked for assessment");

				var connectionIds = selected.Select(c => c.Id).ToList();
				// No calculation is run: §6.4 works from the load effects and the geometry alone. The
				// CBFEM group was the only thing here that needed the engine, and it is mothballed
				// (Services/Cbfem_Mothballed/README.md).
				foreach (var con in selected)
					con.Status = "Checking...";

				// The section map (cross-section id → D/T/fy) is read once for the project and handed
				// to every chapter through ChapterContext: §6.4 identifies chord and braces from it,
				// and any member-level chapter would need the same. Read whenever a chapter is going
				// to run, so no chapter has to fetch it for itself.
				Dictionary<int, Services.Norsok64.JointSectionInfo> sectionMap = new();
				if (selectedChapters.Count > 0)
				{
					try
					{
						var crossSections = await _apiClient.Material.GetCrossSectionsAsync(_projectId, cancellationToken: ct);
						sectionMap = Services.Norsok64.JointSectionMap.FromCrossSections(crossSections.Cast<object>());
						Log($"Section map: {sectionMap.Count} cross-section(s)");
					}
					// A cancelled run is not a failed section map.
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						// Not fatal here: a chapter that needs the map says so itself, with a reason
						// the reader sees in the results (ChapterOutcome.NotPerformed).
						Log($"WARNING: the section map could not be read ({ex.Message})");
					}
				}

				// ── Evaluate Norsok per connection ──
				ShowStatus("Evaluating Norsok N-004 formulas...");
				Log("Evaluating Norsok N-004 formulas...");
				// Drop the results of the connections being re-run ONLY. A blanket Clear() would
				// also erase the stored §6.4 detail of connections the user left unticked, whose
				// rows still show a verdict — the tab and the report would then have a row with
				// nothing behind it.
				foreach (var con in selected)
				{
					_formulaResults.Remove(con.Id);
					_topologyPerConnection.Remove(con.Id);
					// The figure too, keyed by NAME as the report keys it. It is coloured by the
					// envelope utilisations, so a stale one does not merely look old — it states a
					// different result from the table beside it.
					if (con.Name != null) _jointFigures.Remove(con.Name);
				}

				foreach (var con in selected)
				{
					// Per connection: the §6.4 evaluation is the loop that is worth interrupting
					// without the engine in the way.
					ct.ThrowIfCancellationRequested();

					Log($"  ── Connection: {con.Name} ──");
					ShowStatus($"Evaluating: {con.Name}...");

					// Fetch load effects for this connection
					List<ConLoadEffect>? loadEffects = null;
					try
					{
						// isPercentage: false is MANDATORY — a user may have stored load effects as
						// percentages (σ/fy ratios ~0.003); without the flag the service returns them
						// as saved and every downstream check would silently collapse to util≈0.
						loadEffects = await _apiClient.LoadEffect.GetLoadEffectsAsync(_projectId, con.Id, isPercentage: false);

						// Refresh the grid's active/total column from what was just read. It is
						// filled at load time, but the run can change it: with the toggle off,
						// ActivateAllLoadEffectsAsync switches every state on, and a column still
						// reading "4 / 15" would then contradict what was actually assessed.
						con.TotalLoadEffects = loadEffects.Count;
						con.ActiveLoadEffects = loadEffects.Count(le => le.Active);

						// Honour the model's own on/off switches. A load effect the engineer disabled
						// in IDEA StatiCa is one they decided not to design for, so assessing it
						// anyway reports utilisations for a state that is not part of the design —
						// and on an envelope it can make a disabled state the governing one.
						// Per connection, because the switches are per connection.
						//
						// This filters the §6.4 side only. The CBFEM side is calculated by the engine
						// from the project itself, and this client version (26.0.4) exposes no
						// load-effect selector on Calculate — verified: loadEffectIds appears nowhere
						// in its surface — so there is nothing to pass. Whether the engine itself
						// honours the active flags is NOT verified here; if a CBFEM utilisation ever
						// disagrees with the §6.4 set under this toggle, that is the thing to check
						// first.
						if (activeLoadEffectsOnly)
						{
							int total = loadEffects.Count;
							var active = loadEffects.Where(le => le.Active).ToList();
							if (active.Count < total)
								Log($"    Load effects: {active.Count} of {total} active "
									+ $"({total - active.Count} switched off in the model, skipped)");
							else
								Log($"    Load effects: {total} load case(s), all active");
							loadEffects = active;
						}
						else
						{
							int off = loadEffects.Count(le => !le.Active);
							Log($"    Load effects: {loadEffects.Count} load case(s)"
								+ (off > 0 ? $" — including {off} switched off in the model" : ""));
						}

						// Log per member per LC
						foreach (var le in loadEffects)
						{
							foreach (var ml in le.MemberLoadings ?? new())
							{
								if (ml.SectionLoad == null) continue;
								var sl = ml.SectionLoad;
								Log($"      LC[{le.Id}] Member {ml.MemberId}: N={sl.N:F0} Vy={sl.Vy:F0} Vz={sl.Vz:F0} Mx={sl.Mx:F0} My={sl.My:F0} Mz={sl.Mz:F0}");
							}
						}
					}
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						Log($"    WARNING: Could not fetch load effects: {ex.Message}");
					}

					// ── Every selected chapter, in turn ──
					//
					// The run no longer knows how any chapter works: each one prepares its own inputs
					// and returns its rows. Adding a chapter is a new IChapter plus a line in the
					// registry — this loop does not change. (ChapterRegistryTests holds the app to
					// that.)
					var context = new Services.Chapters.ChapterContext
					{
						Client = _apiClient,
						ProjectId = _projectId,
						ConnectionId = con.Id,
						ConnectionName = con.Name,
						LoadEffects = loadEffects,
						// The count BEFORE the active-only filter above, so a chapter can tell
						// "every state is switched off" from "no state is defined" — the same to a
						// chapter, different facts about the model. con.TotalLoadEffects is set
						// before the filter runs, which is why it is read from there.
						LoadEffectsInFile = con.TotalLoadEffects,
						SectionMap = sectionMap,
						Log = Log,
					};

					var formulaResults = new List<NorsokFormulaResult>();
					foreach (var chapter in selectedChapters)
					{
						var outcome = await chapter.EvaluateAsync(context, ct);
						formulaResults.AddRange(outcome.Rows);

						// A chapter that deliberately skipped a check says so where the results are
						// read, not only in its own log — see ChapterOutcome.NotPerformed.
						foreach (var np in outcome.NotPerformed)
							Log($"    {chapter.Key} NOT PERFORMED — {np.What}: {np.Why}");
					}

					_formulaResults[con.Id] = formulaResults;

					// The rules themselves are in CheckWorkflow.Roll — a pure function, so the app's
					// headline verdict can be tested without a window. Logging stays here because it
					// is a UI concern; the roll-up must not know about a log.
					foreach (var fr in formulaResults)
					{
						if (fr.IsNote)
							Log($"    {fr.Section} NOTE: {fr.CheckExpression}");
						else
							Log($"    {fr.Section} {fr.Title}: util={fr.Utilization * 100:F1}% {fr.Verdict}");
					}

					var verdict = Services.CheckWorkflow.Roll(formulaResults);
					con.NorsokPass = verdict.Pass;
					con.MaxUtilization = verdict.MaxUtilisation;
					con.Status = verdict.Status;

					if (verdict.Pass == "N/A")
						Log("    nothing was assessed for this connection — reported as N/A, not as a pass"
							+ "; see the Results tab for the conditions that were not met");
					else if (verdict.Pass == "PARTIAL")
						Log("    part of this connection was not assessed");

					// The report's joint figure, made here while this connection's bodies are in
					// hand — and only if something was assessed, which the verdict above now says.
					await RenderJointFigureAsync(con.Id);
				}

				// Each tab is filled from here, in turn. PopulateResultsTab used to call the other two
				// itself, which made the §6.4 tab impossible to refresh without refreshing Results —
				// the opposite of what Results is: a read-only summary of what the others computed.
				PopulateResultsTab();
				PopulateJoint64Tab();
				PopulateReportTab();

				TabResults.IsEnabled = true;
				TabReport.IsEnabled = true;
				Log("Norsok check completed.");

				// Reports on what this run assessed. Measuring "all passed" over the whole project
				// would let an untouched connection from an earlier run decide the verdict.
				Telemetry.CheckCompleted(
					allPassed: selected.All(c => c.NorsokPass == "PASS"),
					governingUtilization: selected.Max(c => c.MaxUtilization));
			}
			// Cancelling is a decision, not a failure: no error dialog, no failure telemetry.
			// Whatever finished before the stop keeps its verdict; the rest is left as it stood,
			// which is why the log says the results are partial rather than pretending it ran.
			catch (OperationCanceledException)
			{
				Log("Check cancelled. Results are partial — connections not reached keep their previous state.");
				foreach (var con in selected)
				{
					if (con.Status == "Calculating...")
						con.Status = "Cancelled";
				}
			}
			catch (Exception ex)
			{
				Telemetry.CheckFailed(ex);
				AppLog.ReportFailure("The NORSOK check failed", ex);
				Log($"ERROR: {ex.Message}");
				MessageBox.Show(ex.Message, "Check Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				BtnRunCheck.IsEnabled = true;
				BtnCancelCheck.Visibility = Visibility.Collapsed;
				BtnCancelCheck.IsEnabled = false;
				_checkCts?.Dispose();
				_checkCts = null;
				HideStatus();
			}
		}

		/// <summary>
		/// Read how many load effects each connection has, and how many are switched on, so the
		/// connections table can show it before anything is run — that count is what the
		/// "Active load effects only" toggle decides between.
		///
		/// A connection whose load effects cannot be read keeps its counts unset (-1), which the
		/// grid shows as an em dash: not knowing is not the same as having none.
		/// </summary>

	}
}
