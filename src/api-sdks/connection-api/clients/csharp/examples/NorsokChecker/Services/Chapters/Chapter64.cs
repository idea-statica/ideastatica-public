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

		/// <summary>
		/// How the chapter obtains the connection's members. Null in the app, which reads them from
		/// the service; set by a test that needs the branches AFTER the member fetch.
		///
		/// It exists because those branches were unreachable offline. The member read is not
		/// optional and its failure is caught as a blocked input, so a test with no service either
		/// supplied an empty section map — which blocks the chapter before the load effects are
		/// looked at — or handed in a null client, whose NullReferenceException the catch turns into
		/// the very same blocked input. Either way the three "nothing to check" cases could not be
		/// measured, which is how a swap of two of their sentences stayed invisible.
		/// </summary>
		internal Func<ChapterContext, CancellationToken, Task<List<JointMemberData>>>? MembersSource
		{ get; set; }

		public async Task<ChapterOutcome> EvaluateAsync(ChapterContext ctx, CancellationToken ct)
		{
			// THE MEMBERS FIRST, then the load effects — the order decides which reason the reader
			// is given, and it used to be the wrong way round.
			//
			// Measured on CON10 of the shipped test project: it holds ONE member, M2, continuous. No
			// brace. §6.4 checks a brace against a chord, so with no brace there is no d, hence no
			// β = d/D, no θ, no Q_u — equations (6.52), (6.53) and (6.57) are undefined rather than
			// unsatisfied. That is a permanent property of the geometry.
			//
			// Reading the load effects first reported a CONSEQUENCE instead: the connection's
			// inherited load effects still reference the deleted braces, so the service answers 404
			// ("The given key '1' was not present in the dictionary"), and the report said "the load
			// effects of this connection could not be read". True, but it is not why no check is
			// possible — and as a NotEvaluated it told the reader to fix the model and re-run, which
			// cannot help: there is nothing here for §6.4 to assess whatever the load effects say.
			List<JointMemberData>? members = null;
			string? blocked = null;

			if (MembersSource != null)
			{
				members = await MembersSource(ctx, ct);
			}
			else if (ctx.SectionMap.Count == 0)
			{
				blocked = "no cross-section data was available for the project";
			}
			else
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

			// A joint with no BRACE is outside §6.4 by geometry, and it is decidable from the members
			// alone — before the load effects, whose own failure is a consequence of the same edit.
			//
			// §6.4 checks a brace against a chord: with no brace there is no d, so no β = d/D, no θ,
			// no Q_u, and eq (6.52)/(6.53)/(6.57) are undefined rather than unsatisfied. Nothing a
			// reader does to the model changes that, which is why it is OutsideScope and not
			// NotEvaluated — the latter tells them to fix the input and run again.
			//
			// The topology says the same thing ("No brace (chord only).", JointTopologyBuilder.cs:493)
			// but never got the chance on CON10: the load-effect 404 blocked the chapter first.
			if (blocked == null && members is { Count: > 0 } && !members.Any(m => !m.IsContinuous))
			{
				// `Count: > 0` matters: an EMPTY member list also has no brace, but "one continuous
				// member and no brace" would be a false description of it. An empty connection falls
				// through to the topology, which reports it on its own terms.
				return OutsideScope(members.Count == 1
					? $"the connection holds one continuous member ({members[0].Name}) and no brace "
						+ "— §6.4 checks a brace against a chord, so there is nothing to assess"
					: $"the connection has {members.Count} continuous members and no brace "
						+ "— §6.4 checks a brace against a chord");
			}

			// A geometrically valid joint with NO load effect to check.
			//
			// Its own answer, because it is neither of the other two. The model read perfectly well
			// — there is simply nothing in it to assess, either because no load effect is defined or
			// because every one is switched off (the app filters to active-only by default, so a
			// joint whose states are all disabled arrives here with an empty list). That is a
			// legitimate state of a model someone is still working on, not a failure, and the reader
			// needs to be told which of the two it is.
			//
			// It used to fall into the blocked-input row below and report "the model could not be
			// read", which is false: nothing failed. Kept as NotEvaluated all the same — unlike the
			// brace-less case, this one IS fixed by editing the model, which is exactly what that
			// state tells the reader to do.
			if (blocked == null && (ctx.LoadEffects == null || ctx.LoadEffects.Count == 0))
			{
				// THREE distinct facts about the model, and the API tells them apart even though all
				// three arrive here as "nothing to check":
				//   404          -> unreadable (CON10: states reference deleted members)
				//   200 + []     -> none was ever defined
				//   200 + n rows, every one active=false -> the engineer switched them off
				// The third is invisible without LoadEffectsInFile, because the app filters to
				// active-only before a chapter sees anything. Telling an engineer who deliberately
				// disabled every state that their model has no load effect would be wrong about
				// their model and would send them looking for something that is not missing.
				// ONE decision, from which the reason, the title and the sentence are all derived.
				// They used to be three independent conditionals over the same two facts, and the
				// roll-up then recovered the reason by matching the sentence — so a reworded
				// sentence changed a verdict, and swapping two of them swapped two verdicts with
				// every test still green. The enum is the carrier now; the text is display only.
				NotAssessedReason reason =
					ctx.LoadEffects == null ? NotAssessedReason.Unreadable
					: ctx.LoadEffectsInFile > 0 ? NotAssessedReason.AllSwitchedOff
					: NotAssessedReason.NoLoadEffectDefined;

				string why = reason switch
				{
					NotAssessedReason.Unreadable =>
						"the load effects of this connection could not be read",
					NotAssessedReason.AllSwitchedOff =>
						$"all {ctx.LoadEffectsInFile} load effect(s) of this connection are switched "
							+ "off in the model — switch one on, or run with 'active load effects "
							+ "only' unticked",
					_ => "this connection has no load effect defined",
				};

				return new ChapterOutcome
				{
					Rows = new[]
					{
						new NorsokFormulaResult
						{
							Section = "6.4",
							Equation = "",
							Title = reason switch
							{
								NotAssessedReason.Unreadable => "Could not be evaluated",
								NotAssessedReason.AllSwitchedOff => "All load effects switched off",
								_ => "No load effect defined",
							},
							CheckExpression = why,
							Formula = "-",
							FormulaSubstituted = "no §6.4 check was performed for this joint",
							NotAssessed = true,
							Reason = reason,
						},
					},
					NotPerformed = new[] { new NotPerformed("§6.4 tubular joint check", why) },
				};
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
		/// One row saying §6.4 does not COVER this joint — a permanent property of its geometry.
		///
		/// Distinct from the blocked-input row above in the one way that matters to a reader: there
		/// is nothing to fix. "Not evaluated" asks them to correct the model and run again; this says
		/// the chapter will never apply, so another method is needed.
		/// </summary>
		private static ChapterOutcome OutsideScope(string reason) => new()
		{
			Rows = new[]
			{
				new NorsokFormulaResult
				{
					Section = "6.4",
					Equation = "",
					Title = "Outside the scope of §6.4",
					CheckExpression = reason,
					Formula = "-",
					FormulaSubstituted = "no §6.4 check was performed for this joint",
					NotAssessed = true,
					Reason = NotAssessedReason.OutsideScope,
				},
			},
			NotPerformed = new[] { new NotPerformed("§6.4 tubular joint check", reason) },
		};

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
