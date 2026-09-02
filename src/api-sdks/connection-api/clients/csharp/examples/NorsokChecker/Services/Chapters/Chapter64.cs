using NorsokChecker.Models;
using NorsokChecker.Services.Norsok64;

namespace NorsokChecker.Services.Chapters
{
	/// <summary>
	/// NORSOK N-004 §6.4 — tubular joints.
	///
	/// The chapter owns everything from the connection's members to its check rows: building the
	/// typed members, overriding their D/T from the model, deriving the topology, and running the
	/// engine. That preparation used to be spread through the run — 60 lines of it — where it read
	/// as part of the app rather than as part of the chapter, and where a second chapter would have
	/// had to be interleaved with it.
	///
	/// The topology is handed back through <see cref="Topology"/> because the §6.4 tab shows any
	/// single load effect, not just the envelope the result rows carry.
	/// </summary>
	internal sealed class Chapter64 : IChapter
	{
		public string Key => "6.4";
		public string DisplayName => "§6.4 Joints";
		public string ReportGroup => "§6.4 Tubular joints";
		public bool HasOwnTab => true;

		/// <summary>
		/// Called with the joint topology when one was built, so the §6.4 tab can bind it. Set by the
		/// run before evaluating; null for a caller that only wants the rows (a test, or the report).
		/// </summary>
		internal Action<int, JointTopology>? Topology { get; set; }

		public async Task<ChapterOutcome> EvaluateAsync(ChapterContext ctx, CancellationToken ct)
		{
			// Why §6.4 is not going to run, when it is not going to run. Each of these used to leave
			// the chapter silently absent, and a connection with no §6.4 row could then read PASS off
			// whatever else it had. Reachable in the shipped test set: CON10's braces are deleted, so
			// its inherited load effects reference members that no longer exist and the service
			// answers 404.
			string? blocked =
				ctx.LoadEffects == null ? "the load effects of this connection could not be read"
				: ctx.LoadEffects.Count == 0 ? "this connection has no load effect — nothing to check"
				: ctx.SectionMap.Count == 0 ? "no cross-section data was available for the project"
				: null;

			List<JointMemberData>? members = null;
			if (blocked == null)
			{
				try
				{
					var conMembers = await ctx.Client.Member.GetMembersAsync(
						ctx.ProjectId, ctx.ConnectionId, cancellationToken: ct);
					members = conMembers
						.Select(m => JointMemberData.FromConMember(
							m, ctx.SectionMap.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
						.ToList();

					// D/T from the connection's OWN model, not from the section name. The section map
					// is per project and name-derived, which is wrong for 96 % of catalogue circular
					// profiles; the IOM facet ring is per connection and measured. See TubeFromIom.
					await EnrichFromIomAsync(ctx, members, ct);
				}
				catch (OperationCanceledException) { throw; }
				catch (Exception ex)
				{
					// No fallback exists, so this is a GAP, not a degraded check.
					ctx.Log($"    WARNING: §6.4 member fetch failed ({ex.Message}) "
						+ "— no §6.4 check was performed for this connection");
					blocked = ex.Message;
					members = null;
				}
			}

			if (blocked != null || members == null)
			{
				return new ChapterOutcome
				{
					Rows = new[]
					{
						new NorsokFormulaResult
						{
							Section = "6.4",
							// EMPTY, not "6.4.3": nothing was evaluated here, so there is no equation to
							// name — and 6.4.3 is a CLAUSE, which printed as "(Eq. 6.4.3)" states
							// something the norm does not contain. The card suppresses the badge when
							// this is empty.
							Equation = "",
							// No "§6.4" prefix: the card already prints §{Section} beside the title, so
							// carrying it here rendered "§6.4 §6.4 could not be evaluated".
							Title = "Could not be evaluated",
							CheckExpression = $"the joint's members could not be read: {blocked}",
							Formula = "-",
							FormulaSubstituted = "no §6.4 check was performed for this joint",
							NotAssessed = true,
							// NOT "outside scope": nothing here says the chapter fails to cover this
							// joint. The inputs could not be produced, so the reader's move is to fix
							// the model and run again. The overview row used to say the opposite.
							Reason = NotAssessedReason.NotEvaluated,
						},
					},
					NotPerformed = new[] { new NotPerformed("§6.4 tubular joint check", blocked ?? "unknown") },
				};
			}

			var rows = new List<NorsokFormulaResult>();
			var runner = new NorsokCheckRunner(ctx.Client, ctx.ProjectId, ctx.Log);
			bool assessed = runner.EvaluateJointChecksFromTopology(
				members, ctx.LoadEffects, rows,
				topology: t => Topology?.Invoke(ctx.ConnectionId, t));

			// A joint that fails the §6.4 conditions is not assessed per brace either: the quantities
			// the check rests on (the joint plane, the averaged chord stresses, the K/Y/X balance) are
			// not meaningful, so nothing downstream is published. The rejection rows are already in
			// `rows` — the topology publishes one per unmet condition.
			if (!assessed)
				ctx.Log("    §6.4 topology rejected the joint — no §6.4 check is performed");

			return new ChapterOutcome { Rows = rows };
		}

		/// <summary>
		/// Measured D/T from the IOM facet ring, replacing whatever the section name implied.
		///
		/// A failure here is logged and tolerated: the name-derived values remain, which is worse but
		/// not nothing. An export that comes back EMPTY is different from a model without tubes, and
		/// saying "no tubular beams" for it would be a false statement about the user's model —
		/// measured 2026-08-27, the 26.0.4 client deserialises 26.1's payload to null while the
		/// service returns it in full.
		/// </summary>
		private static async Task EnrichFromIomAsync(
			ChapterContext ctx, List<JointMemberData> members, CancellationToken ct)
		{
			IdeaRS.OpenModel.Connection.ConnectionData? iom;
			try
			{
				iom = await ctx.Client.Export.ExportIomConnectionDataAsync(
					ctx.ProjectId, ctx.ConnectionId, cancellationToken: ct);
			}
			catch (OperationCanceledException) { throw; }
			catch (Exception ex)
			{
				ctx.Log($"    WARNING: IOM export failed ({ex.Message}) — D/T stay as parsed from the section names");
				return;
			}

			if (iom?.Beams == null || iom.Beams.Count == 0)
			{
				ctx.Log("    WARNING: the IOM export returned no model for this connection"
					+ $" ({(iom == null ? "no data at all" : "no beams")})"
					+ " — D/T cannot be read from the model, so any tube whose section name does not"
					+ " spell out its dimensions will be reported as unreadable");
				return;
			}

			var beams = TubeFromIom.TubularBeamsByName(iom);
			if (beams.Count == 0)
			{
				ctx.Log($"    IOM: the model has {iom.Beams.Count} beam(s) but none of a tubular type"
					+ " — D/T stay as parsed from the section names");
				return;
			}

			foreach (var m in members)
			{
				// only tubular members: the facet formula would return a plausible-looking number for
				// an I-section too, and that is worse than no number at all
				if (m.Section == null || !beams.TryGetValue(m.Name ?? "", out var beam)) continue;

				var (d, t, why) = TubeFromIom.FromBeam(beam);
				if (d is not > 0 || t is not > 0)
				{
					ctx.Log($"    IOM: '{m.Name}' D/T not readable ({why}) — keeping the name-derived values");
					continue;
				}

				// The name and the model disagreeing by more than 2 % is worth saying: "PIPE127STD" is
				// really Ø141.3, because 127 is the nominal size. Python calls this geom_note.
				if (m.Section.D is > 0 && Math.Abs(m.Section.D.Value - d.Value) / m.Section.D.Value > 0.02)
					m.Section.GeomNote = $"section name says Ø{m.Section.D:F1}, the model measures Ø{d:F1}";

				m.Section.D = d;
				m.Section.T = t;
			}
		}
	}
}
