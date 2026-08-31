using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using NorsokChecker.Models;
using NorsokChecker.Services.Norsok64;

namespace NorsokChecker
{
	/// <summary>
	/// The §6.4 tab: the per-brace check table for one connection, in either the envelope or a
	/// single load effect, with the joint drawn in its own plane beside it.
	///
	/// A separate partial because MainWindow.xaml.cs is already 1000 lines of run/plumbing and this
	/// is presentation over data that is already computed — the topology kept per connection by
	/// EvaluateJointChecksFromTopology.
	///
	/// Shaped after the python reference (ui.html renderResults), with two deliberate departures:
	///   - the derivation is a window rather than a modal, because WPF has no modal-in-page idiom;
	///   - the K/X/Y columns are in that order everywhere. Python's table uses K, X, Y while its
	///     detail card uses K, Y, X; that inconsistency is not worth copying.
	/// </summary>
	public partial class MainWindow
	{
		/// <summary>Guard: the selector handlers fire while the tab is being populated.</summary>
		private bool _joint64Loading;

		/// <summary>
		/// The column groups of the classification table: a label and the columns it spans, by index
		/// into <c>Grid64.Columns</c>.
		///
		/// Indices rather than header names because the headers are now the bare column names ("K",
		/// "N_Rd", "axial") and two groups could legitimately contain a column with the same name.
		/// Brace (0), Governing LC (1) and Notes (14) sit outside any group and get no banner cell.
		///
		/// Verified against the grid's real DisplayIndex order, not counted off the XAML: the source
		/// nests a template column inside the Brace column, so reading the file top to bottom gives
		/// an order that is one out. TheGroupBandSpansTheRightColumns pins it.
		/// </summary>
		private static readonly (string Label, int First, int Last)[] Group64Spans =
		{
			("Classification", 2, 4),            // K, X, Y
			("Resistance", 5, 7),                // N_Rd, M_Rd,ip, M_Rd,op
			("Utilisation breakdown", 8, 10),    // axial, in-plane, out-of-plane
			("Check", 11, 13),                   // utilisation, flags, Verdict
		};

		/// <summary>
		/// Lay out the group banner over the classification table: one centred label per group,
		/// spanning exactly the columns of that group, divided by the same rules the cells carry.
		///
		/// Done in code and re-run on every layout change because a group heading has to span
		/// columns, and WPF's DataGrid has no colspan — the banner is a separate Canvas that has to
		/// be kept in step with column widths that change with their content (SizeToHeader) and with
		/// the grid's horizontal scroll offset. The previous attempt put the group name on the first
		/// line of the group's FIRST column header, which reads as a label for that one column
		/// instead of a heading over three.
		/// </summary>
		/// <summary>
		/// Keep the banner aligned for the life of the window. Hooked to LayoutUpdated rather than
		/// called once after binding, because every one of these moves the columns and each would
		/// otherwise leave the banner behind: the window resized, the splitter dragged, a column
		/// auto-sized to a longer number, the table scrolled sideways, the tab shown for the first
		/// time (nothing has a width until then).
		/// </summary>
		private void HookGroup64Band()
		{
			Grid64.LayoutUpdated += (_, _) => SyncGroup64Band();
		}

		/// <summary>
		/// The geometry the banner was last drawn for. Rebuilding the Canvas dirties the layout, which
		/// raises LayoutUpdated again — without this guard the handler would loop forever, pegging a
		/// core and never settling. Comparing the widths and the scroll offset makes the rebuild
		/// happen only when something actually moved.
		/// </summary>
		private string _group64BandKey = "";

		private void SyncGroup64Band()
		{
			if (Group64Band == null || Grid64 == null) return;
			if (Grid64.Columns.Count <= Group64Spans.Max(g => g.Last)) return;   // grid not built yet

			string key = string.Join(",", Grid64.Columns
					.OrderBy(c => c.DisplayIndex)
					.Select(c => c.ActualWidth.ToString("F1")))
				+ "|" + HorizontalOffsetOf(Grid64).ToString("F1");
			if (key == _group64BandKey) return;
			_group64BandKey = key;

			Group64Band.Children.Clear();

			// x of each column's left edge, from the display order the grid actually uses
			var ordered = Grid64.Columns.OrderBy(c => c.DisplayIndex).ToList();
			var left = new double[ordered.Count + 1];
			for (int i = 0; i < ordered.Count; i++)
				left[i + 1] = left[i] + ordered[i].ActualWidth;

			double scroll = HorizontalOffsetOf(Grid64);

			foreach (var (label, first, last) in Group64Spans)
			{
				if (last >= ordered.Count) continue;

				double x = left[first] - scroll;
				double width = left[last + 1] - left[first];
				if (width <= 0) continue;

				var cell = new System.Windows.Controls.Border
				{
					Width = width,
					Height = Group64Band.Height,
					// left rule only: the next group's cell draws the boundary on its own left, so a
					// right rule would double every internal line
					BorderBrush = new System.Windows.Media.SolidColorBrush(
						System.Windows.Media.Color.FromRgb(0xB0, 0xBE, 0xC5)),
					BorderThickness = new Thickness(1, 0, 0, 0),
					Child = new TextBlock
					{
						Text = label,
						FontSize = 10.5,
						Foreground = new System.Windows.Media.SolidColorBrush(
							System.Windows.Media.Color.FromRgb(0x54, 0x6E, 0x7A)),
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						TextTrimming = TextTrimming.CharacterEllipsis,
						ToolTip = label,
					},
				};
				System.Windows.Controls.Canvas.SetLeft(cell, x);
				System.Windows.Controls.Canvas.SetTop(cell, 0);
				Group64Band.Children.Add(cell);
			}
		}

		/// <summary>
		/// The horizontal scroll offset of a DataGrid's internal ScrollViewer, or 0.
		///
		/// The viewer is found once and kept: this is called from a LayoutUpdated handler, and walking
		/// the whole visual tree of a populated grid on every layout pass is not free.
		/// </summary>
		private System.Windows.Controls.ScrollViewer? _grid64Scroll;

		private double HorizontalOffsetOf(DependencyObject grid)
		{
			_grid64Scroll ??= FindVisualChild<System.Windows.Controls.ScrollViewer>(grid);
			return _grid64Scroll?.HorizontalOffset ?? 0;
		}

		private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
		{
			int n = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < n; i++)
			{
				var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
				if (child is T hit) return hit;
				var deeper = FindVisualChild<T>(child);
				if (deeper != null) return deeper;
			}
			return null;
		}

		/// <summary>
		/// Draw the utilisation legend from <see cref="UtilisationScale"/> — one swatch per band, in
		/// order. Built in code rather than listed in the XAML so the key cannot disagree with the
		/// scale the view and the rows actually use; the four hand-written swatches it replaced had
		/// to be edited in step with two other copies of the ramp.
		///
		/// The swatches show the FLAT tones. The 3D bodies are painted with the lit variants, which
		/// are lighter on purpose so that they land near these once the viewport's lighting has
		/// multiplied them — comparing a body against this key is the point of it.
		/// </summary>
		private void BuildUtilisationLegend()
		{
			Legend64Swatches.Children.Clear();
			for (int band = 0; band < UtilisationScale.BandCount; band++)
			{
				bool over = band == UtilisationScale.BandCount - 1;
				var colour = UtilisationScale.Parse(UtilisationScale.HexOfBand(band));
				double from = band / (double)UtilisationScale.RampBandCount;
				double to = (band + 1) / (double)UtilisationScale.RampBandCount;
				Legend64Swatches.Children.Add(new System.Windows.Controls.Border
				{
					// narrower than the four swatches this replaced, so eleven take about the room
					// the old four did
					Width = 9,
					Height = 9,
					Background = new System.Windows.Media.SolidColorBrush(colour),
					// a gap before the over-capacity swatch: it is a different KIND of statement from
					// the ten that divide capacity, and butting it against them reads as an 11th tenth
					Margin = new Thickness(over ? 3 : 0, 0, 1, 0),
					ToolTip = over ? "at or over 100 % — overloaded" : $"{from * 100:F0}–{to * 100:F0} %",
				});
			}
		}

		/// <summary>
		/// Put one topology on the tab as a completed run would, so a test can read back what the
		/// sheet decided — visibility, the summary line, the grid's contents.
		///
		/// Exists because the alternative is a test that restates the tab's own condition, and such
		/// a test keeps passing when the tab changes. Measured twice on 2026-08-27: two fixtures
		/// written that way stayed green while the production code they guarded was reverted.
		/// </summary>
		internal void SetJoint64TopologyForTest(int connectionId, string name, JointTopology topo)
		{
			_connections.Add(new ConnectionCheckResult { Id = connectionId, Name = name });
			_topologyPerConnection[connectionId] = topo;
			PopulateJoint64Tab();
		}

		/// <summary>Fill the §6.4 tab's selectors from the connections that produced a topology.</summary>
		private void PopulateJoint64Tab()
		{
			_joint64Loading = true;
			try
			{
				Cmb64Connection.Items.Clear();
				foreach (var con in _connections)
				{
					if (!_topologyPerConnection.ContainsKey(con.Id)) continue;
					Cmb64Connection.Items.Add(new ComboBoxItem { Content = con.Name, Tag = con.Id });
				}
				Tab64.IsEnabled = Cmb64Connection.Items.Count > 0;
				if (Cmb64Connection.Items.Count > 0)
					Cmb64Connection.SelectedIndex = 0;
			}
			finally
			{
				_joint64Loading = false;
			}
			// rebuildLeList: the load-effect list has to be built HERE. Setting SelectedIndex above
			// does fire Joint64Selection_Changed, but _joint64Loading is still true at that point so
			// the handler returns immediately — this call is the only chance the list gets. Without
			// it Cmb64Le stayed empty after a run, and switching to "per LC" then had nothing to
			// select and showed no table at all.
			RefreshJoint64(rebuildLeList: true);
		}

		private void Joint64Selection_Changed(object sender, RoutedEventArgs e)
		{
			if (_joint64Loading || !IsLoaded) return;

			// A connection change has to rebuild the load-effect list before the table is drawn;
			// a mode or load-effect change must not, or the rebuild would reset the selection.
			if (ReferenceEquals(sender, Cmb64Connection))
				RefreshJoint64(rebuildLeList: true);
			else
				RefreshJoint64();
		}

		private JointTopology? SelectedJoint64Topology()
		{
			if (Cmb64Connection.SelectedItem is not ComboBoxItem { Tag: int conId }) return null;
			return _topologyPerConnection.GetValueOrDefault(conId);
		}

		private void RefreshJoint64(bool rebuildLeList = false)
		{
			var topo = SelectedJoint64Topology();
			if (topo == null)
			{
				Grid64.ItemsSource = null;
				Pnl64Verdict.Visibility = Visibility.Collapsed;
				Lbl64Summary.Text = "";
				return;
			}

			bool envelope = Rb64Envelope.IsChecked == true;
			Cmb64Le.IsEnabled = !envelope;
			Lbl64Le.Opacity = envelope ? 0.5 : 1.0;

			if (rebuildLeList) RebuildLe64List(topo);

			ShowJoint64Verdict(topo);

			// A joint outside the scope of §6.4 has NO results, so the sheet must not show any.
			//
			// It used to draw the whole sheet anyway, and the effect was a flat contradiction on one
			// screen: the banner said "no brace can be assessed" while the table below it listed
			// braces as PASS with utilisations, and the summary counted them as "3 assessed". The
			// numbers were real arithmetic, but they were computed from quantities the rejected
			// condition makes meaningless — a chord that is not tubular has no chord-wall check, and
			// its neighbours' K balance is derived from a joint plane that was never valid. Showing
			// them is the same defect as reporting 0.0 % for a brace nothing was checked on.
			//
			// What stays is the banner: the reason, expanded from the one-line Status in the
			// connections table into the specific conditions that were not met.
			bool rejected = topo.Verdict.Status == "ERROR";
			Pnl64Sheet.Visibility = rejected ? Visibility.Collapsed : Visibility.Visible;

			// the mode and load-effect selectors choose between results; with none, they do nothing
			Rb64Envelope.IsEnabled = !rejected;
			Rb64PerLe.IsEnabled = !rejected;
			if (rejected)
			{
				Cmb64Le.IsEnabled = false;
				Lbl64Le.Opacity = 0.5;
				Grid64.ItemsSource = null;
				Grid64Equilibrium.ItemsSource = null;
				Grid64BraceForces.ItemsSource = null;
				Joint3D64.Clear();
				int gates = topo.Verdict.Errors.Count;
				Lbl64Summary.Text = $"not assessed · {gates} condition(s) not met";
				return;
			}

			ShowJoint64Table(topo, envelope);
			ShowJoint64PerLeCards(topo, envelope);
			ShowJoint64Plane(topo);
		}

		/// <summary>
		/// The two per-load-effect cards beside the joint: the node-equilibrium self-check and the
		/// brace forces resolved into the joint plane.
		///
		/// Both are quantities OF a load effect, so an envelope has no single set of them — mixing
		/// states in one force table would invent a load case that does not exist. In envelope mode
		/// they are replaced by a note saying where to look instead, which is what the python
		/// reference does for the same reason.
		/// </summary>
		private void ShowJoint64PerLeCards(JointTopology topo, bool envelope)
		{
			Pnl64EnvNote.Visibility = envelope ? Visibility.Visible : Visibility.Collapsed;
			Pnl64Equilibrium.Visibility = envelope ? Visibility.Collapsed : Visibility.Visible;
			Pnl64BraceForces.Visibility = envelope ? Visibility.Collapsed : Visibility.Visible;
			if (envelope) return;

			int? leId = (Cmb64Le.SelectedItem as Le64Option)?.Id;

			// ── node equilibrium: two rows, ΣF and ΣM, by component ──
			var eq = topo.Equilibrium.FirstOrDefault(r => r.Id == leId);
			if (eq == null)
			{
				Grid64Equilibrium.ItemsSource = null;
			}
			else
			{
				string state = eq.Ok ? "✓" : "⚠";
				Grid64Equilibrium.ItemsSource = new[]
				{
					new
					{
						Quantity = "ΣF [kN]", State = state,
						X = $"{eq.SumF.X / 1e3:F1}", Y = $"{eq.SumF.Y / 1e3:F1}", Z = $"{eq.SumF.Z / 1e3:F1}",
					},
					new
					{
						Quantity = "ΣM [kNm]", State = state,
						X = $"{eq.SumM.X / 1e3:F2}", Y = $"{eq.SumM.Y / 1e3:F2}", Z = $"{eq.SumM.Z / 1e3:F2}",
					},
				};
			}

			// ── brace forces in the joint plane, with the chord's own forces at each footprint ──
			var bf = topo.BraceForces.FirstOrDefault(r => r.Id == leId);
			var cs = topo.ChordStresses.FirstOrDefault(r => r.Id == leId);
			if (bf == null)
			{
				Grid64BraceForces.ItemsSource = null;
				return;
			}

			var csByName = (cs?.Rows ?? new List<ChordStressRow>())
				.GroupBy(r => r.Name).ToDictionary(g => g.Key, g => g.First());

			Grid64BraceForces.ItemsSource = bf.Rows.Select(r =>
			{
				var c = csByName.GetValueOrDefault(r.Name);
				// ⚠ on the name when the brace axis does not lie in the fitted joint plane: M_ip and
				// M_op are then projections, which changes what the two numbers mean.
				bool projected = r.SubNormalDot < 0.985;
				return new
				{
					Brace = r.Name + (projected ? "  ⚠" : ""),
					NSd = $"{r.NSd / 1e3:F1}",
					Mip = $"{r.Mip / 1e3:F2}",
					Mop = $"{r.Mop / 1e3:F2}",
					Vip = $"{r.Vip / 1e3:F1}",
					Vop = $"{r.Vop / 1e3:F1}",
					Mtor = $"{r.Mtor / 1e3:F2}",
					Face = SideLabel(r.Side),
					NChord = c == null ? "—" : $"{c.NChord / 1e3:F1}",
					MipChord = c == null ? "—" : $"{c.MipChord / 1e3:F2}",
					MopChord = c == null ? "—" : $"{c.MopChord / 1e3:F2}",
				};
			}).ToList();
		}

		/// <summary>
		/// The one wording for a chord face, matching the python reference's `sideLbl`. It had three
		/// different wordings across its own UI once ("face +", "+ey face", "(+)"), which made
		/// engineers doubt they meant the same thing — so this app has exactly one.
		/// </summary>
		private static string SideLabel(int side) => side >= 0 ? "+ey face" : "−ey face";

		/// <summary>
		/// The load-effect selector, each entry carrying that state's worst utilisation — the bar
		/// the python selector draws, reduced to a number because a WPF ComboBox item cannot hold a
		/// meaningful one without a template that would obscure the name.
		/// </summary>
		private void RebuildLe64List(JointTopology topo)
		{
			_joint64Loading = true;
			try
			{
				var options = new List<Le64Option>();
				foreach (var le in topo.JointChecks)
				{
					double worst = 0;
					bool anyFail = false;
					foreach (var row in le.Rows)
					{
						// covers BOTH: a skipped brace has no utilisation to be the worst, and no
						// verdict to fail — it must not paint a ✗ on the whole load effect.
						if (row.Skipped) continue;
						if (!double.IsNaN(row.Util) && row.Util > worst) worst = row.Util;
						if (!row.Passed) anyFail = true;
					}
					options.Add(new Le64Option
					{
						Id = le.Id,
						Name = string.IsNullOrEmpty(le.Name) ? $"LE{le.Id}" : le.Name,
						MaxUtil = worst,
						AnyFail = anyFail,
					});
				}
				Cmb64Le.ItemsSource = options;
				if (options.Count > 0) Cmb64Le.SelectedIndex = 0;
			}
			finally
			{
				_joint64Loading = false;
			}
		}

		/// <summary>
		/// The joint's own verdict. An ERROR withholds the whole check — the quantities it rests on
		/// (the plane, the averaged chord stresses, the K/Y/X balance) are properties of the joint,
		/// so no brace can be assessed once any condition fails, not even one whose geometry is
		/// sound. Every unmet condition is listed separately, as the python reference does; joining
		/// them made a joint that failed six gates look like it failed one.
		/// </summary>
		private void ShowJoint64Verdict(JointTopology topo)
		{
			var v = topo.Verdict;
			bool error = v.Status == "ERROR";
			var lines = new List<string>();

			if (error)
			{
				// This banner IS the result for a rejected joint — the rest of the sheet is hidden,
				// so it has to expand the connections table's one-line Status into the specific
				// conditions and say what each one means. It is the only thing the reader gets.
				int n = v.Errors.Count;
				Lbl64VerdictTitle.Text =
					$"✗ Not assessed — this joint is outside the scope of NORSOK §6.4"
					+ (n > 1 ? $" ({n} conditions not met)" : "");
				Lbl64VerdictBody.Text =
					"Section 6.4 covers simple tubular joints. The joint plane, the chord stresses "
					+ "averaged across it and the K/Y/X force balance are properties of the WHOLE "
					+ "joint, so while any condition below is unmet no brace can be assessed — not "
					+ "even one whose own geometry is fine. Nothing is shown below rather than "
					+ "numbers that would be arithmetic on quantities the norm does not define here."
					+ $"\n\nThe joint: {topo.Chord?.Name ?? "no chord identified"}"
					+ (topo.Chord?.Section.Name is { } cs ? $" ({cs})" : "")
					+ $" as the chord, {topo.GapBraces.Count} brace(s)"
					+ (topo.GapBraces.Count > 0
						? " — " + string.Join(", ", topo.GapBraces.Select(b =>
							$"{b.Name} ({b.Section.Name ?? "section unknown"})"))
						: "");
				lines.AddRange(v.Errors.Select(x => "•  " + x));
				if (v.Warnings.Count > 0)
					lines.AddRange(v.Warnings.Select(x => "⚠  " + x));
				Pnl64Verdict.Background = (System.Windows.Media.Brush)FindResource("VerdictFailBg");
				Lbl64VerdictTitle.Foreground = (System.Windows.Media.Brush)FindResource("VerdictFailFg");
			}
			else if (v.Warnings.Count > 0)
			{
				// Warnings do not block: §6.4.3.1's validity ranges are warnings deliberately,
				// because the norm's own rule there is to compute with the parameters clamped and
				// keep the lesser capacity.
				Lbl64VerdictTitle.Text = $"⚠ Checked, with {v.Warnings.Count} assumption(s)";
				Lbl64VerdictBody.Text =
					"The check ran. These do not block it — the 6.4.3.1 validity ranges are "
					+ "warnings because the norm's rule is to compute with the parameters clamped "
					+ "to the range and keep the lower capacity.";
				lines.AddRange(v.Warnings.Select(x => "⚠  " + x));
				Pnl64Verdict.Background = (System.Windows.Media.Brush)FindResource("VerdictPartialBg");
				Lbl64VerdictTitle.Foreground = (System.Windows.Media.Brush)FindResource("VerdictPartialFg");
			}

			Lst64Conditions.ItemsSource = lines;
			Pnl64Verdict.Visibility = lines.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private void ShowJoint64Table(JointTopology topo, bool envelope)
		{
			Col64Gov.Visibility = envelope ? Visibility.Visible : Visibility.Collapsed;

			var rows = new List<Joint64RowView>();
			int? leId = (Cmb64Le.SelectedItem as Le64Option)?.Id;

			foreach (var brace in topo.GapBraces)
			{
				JointCheckRow? row = null;
				string gov = "";

				if (envelope)
				{
					var pick = JointEnvelope.Pick(topo.JointChecks, brace.Name);
					row = pick?.Row;
					gov = pick?.LeName ?? "";
				}
				else if (leId is { } id)
				{
					row = topo.JointChecks.FirstOrDefault(le => le.Id == id)?.Rows
						.FirstOrDefault(r => r.Name == brace.Name);
				}

				var cls = row?.Classification;
				var view = new Joint64RowView
				{
					Brace = brace.Name,
					GoverningLe = gov,
					Detail = row,
				};

				if (row == null)
				{
					view.Verdict = "N/A";
					view.SkipReason = "no result for this brace in this load effect";
					rows.Add(view);
					continue;
				}

				// What this brace was checked FOR, under its name — the tab used to show only how
				// much capacity was used, never the actions that used it.
				if (row.Inputs is { } inp)
					view.Actions = $"N_Sd={inp.NSd / 1e3:F1} kN · M_ip={inp.MipSd / 1e3:F2} kNm"
						+ $" · M_op={inp.MopSd / 1e3:F2} kNm";

				if (cls != null)
				{
					view.FrK = $"{cls.FrK * 100:F0} %";
					view.FrX = $"{cls.FrX * 100:F0} %";
					view.FrY = $"{cls.FrY * 100:F0} %";
					// the classifier's own reason for the split — the single most explanatory field
					// it produces, and it was going nowhere
					view.Note = cls.Note ?? "";
				}

				if (row.Skipped)
				{
					view.Verdict = "N/A";
					view.SkipReason = row.Reason;
					rows.Add(view);
					continue;
				}

				var e = row.Engine;
				view.NRd = row.NoAxialClassification ? "n/a" : $"{row.NRdWeighted / 1e3:F0} kN";
				view.MRdIp = $"{row.MRdIp / 1e3:F1} kNm";
				view.MRdOp = $"{row.MRdOp / 1e3:F1} kNm";
				if (e != null)
				{
					// The three shares of eq (6.57), taken from the engine's own per-class result
					// rather than recomputed here — recomputing would let the table and the
					// derivation drift apart, and the engine already applies the 6.4.3.1 clamping
					// rule that decides which pass the numbers came from.
					var dom = e.PerClass.GetValueOrDefault(
						Enum.TryParse<Joint64Class>(row.DomClass, out var dc) ? dc : Joint64Class.K);
					view.UtilAxial = row.NoAxialClassification ? "n/a" : Pct(dom?.UtilAxialTerm ?? double.NaN);
					view.UtilIpb = Pct(dom?.UtilIpTerm ?? double.NaN);
					view.UtilOpb = Pct(dom?.UtilOpTerm ?? double.NaN);
				}
				view.Util = double.IsInfinity(row.Util) ? "> 999 %" : $"{row.Util * 100:F1} %";
				view.UtilValue = row.Util;          // the number behind the text, for the row colour
				view.Verdict = row.Passed ? "PASS" : "FAIL";

				// ⚠ geometry outside 6.4.3.1 (resistance extrapolated); ⛔ the chord wall has no
				// capacity left at this footprint, which forces FAIL whatever the number says
				string flags = "";
				if (!row.WithinRange) flags += "⚠";
				if (row.ChordOverstressed) flags += "⛔";
				view.Flags = flags;

				rows.Add(view);

				// One indented sub-row per K component, as the python table has: which neighbour this
				// K fraction balances against, and across which gap. A brace's K share is a SUM over
				// its pairings, and the sum alone cannot say whether it is one strong pairing or
				// three weak ones — the partner and the gap are what make the number checkable.
				foreach (var kc in cls?.KComponents ?? new List<KComponent>())
				{
					double force = kc.Frac * Math.Abs(cls!.NSd);
					rows.Add(new Joint64RowView
					{
						IsSubRow = true,
						Brace = $"↳ K via {kc.Partner}",
						FrK = $"{kc.Frac * 100:F1} %",
						Note = $"{force / 1e3:F1} kN balanced across a "
							+ (kc.GapM is { } g ? $"{g * 1000:F0} mm gap" : "gap of unknown size"),
					});
				}
			}

			Grid64.ItemsSource = rows;

			// braces only: a K sub-row is a breakdown of a brace, not another brace
			var braceRows = rows.Where(r => !r.IsSubRow).ToList();
			int assessed = braceRows.Count(r => r.Verdict is "PASS" or "FAIL");
			int failed = braceRows.Count(r => r.Verdict == "FAIL");
			string where = envelope
				? $"envelope over {topo.JointChecks.Count} load effect(s)"
				: (Cmb64Le.SelectedItem as Le64Option)?.Name ?? "—";
			Lbl64Summary.Text = $"{braceRows.Count} brace(s) · {assessed} assessed · {failed} failed · {where}";

			Lbl64Legend.Text =
				"The axial force in each brace splits into K (balanced against a neighbour across a "
				+ "gap), X (through the chord to the opposite side) and Y (beam shear). The check is "
				+ "the chord-wall check at each brace footprint, eq (6.57). "
				+ (envelope ? "Envelope: each brace reports the load effect that governs IT, so "
					+ "different braces may name different states. " : "")
				+ "⚠ geometry outside the 6.4.3.1 validity range, resistance extrapolated. "
				+ "⛔ the chord wall has no capacity left at that footprint — forced FAIL regardless "
				+ "of the number. Double-click a row for the derivation.";
		}

		private static string Pct(double v) => double.IsNaN(v) ? "—" : $"{v * 100:F1} %";

		/// <summary>
		/// Draw the selected connection in its own joint plane. Uses the meshes already cached for
		/// the Check tab, and turns the camera rather than the model — two model angles cannot bring
		/// an arbitrary plane normal onto this camera's oblique line of sight (measured: a joint in
		/// the global XY plane reached only |dot| = 0.84). See Joint3DView.LookAtPlane.
		/// </summary>
		private async void ShowJoint64Plane(JointTopology topo)
		{
			// Read as a drawing of the joint plane, so the mouse must not turn it — see
			// Joint3DView.Interactive. Turning is offered as 90-degree steps and a normal flip.
			Joint3D64.Interactive = false;
			// names on the bodies, as the python schematic labels its members — with the table
			// beside it no longer highlighting on hover, the label is how a body is identified
			Joint3D64.ShowMemberLabels = true;

			if (Cmb64Connection.SelectedItem is not ComboBoxItem { Tag: int conId } item) return;
			Lbl64JointTitle.Text = $"Joint — {item.Content}";

			// Fetch on demand rather than reading the cache: it is filled from the Check tab, so a
			// connection never selected there had no bodies here and the view said "0 members"
			// beside a full set of tables.
			var meshes = await MeshesForAsync(conId, item.Content?.ToString());

			// The selection can change while the fetch is in flight — a slow presentation payload
			// for CON8 must not paint itself over CON9 once the user has moved on.
			if (Cmb64Connection.SelectedItem is not ComboBoxItem { Tag: int stillSelected }
				|| stillSelected != conId) return;

			if (meshes.Count == 0)
			{
				Joint3D64.Clear();
				return;
			}

			Joint3D64.Load(meshes);
			var n = topo.NPlane;
			var ex = topo.Ex;
			if (n.Norm > 1e-9)
				Joint3D64.LookAtPlane(new Vector3D(n.X, n.Y, n.Z), new Vector3D(ex.X, ex.Y, ex.Z));

			ColourJoint64ByResults(topo);
		}

		/// <summary>
		/// Paint the joint by what the check said, so the picture and the table agree — the same
		/// utilisation that colours a table row colours that brace's body.
		///
		/// The utilisation used is the one the table is showing: the governing state per brace in
		/// envelope mode, the selected state in per-LC mode. A brace with no result is grey, not
		/// green — an unchecked member coloured "safe" is the same mistake as a 0.0 % utilisation
		/// standing in for "not assessed".
		/// </summary>
		private void ColourJoint64ByResults(JointTopology topo)
		{
			bool envelope = Rb64Envelope.IsChecked == true;
			int? leId = (Cmb64Le.SelectedItem as Le64Option)?.Id;

			var utilByMember = new Dictionary<int, double?>();
			foreach (var brace in topo.GapBraces)
			{
				JointCheckRow? row = envelope
					? JointEnvelope.Pick(topo.JointChecks, brace.Name)?.Row
					: topo.JointChecks.FirstOrDefault(le => le.Id == leId)?.Rows
						.FirstOrDefault(r => r.Name == brace.Name);

				utilByMember[brace.Id] = (row == null || row.Skipped || double.IsNaN(row.Util))
					? null
					: row.Util;
			}

			Joint3D64.ColourByUtilisation(utilByMember, topo.Chord?.Id ?? -1);
		}

		private void Joint64_RotateLeft(object sender, RoutedEventArgs e) => Joint3D64.TurnInPlane(-90);
		private void Joint64_RotateRight(object sender, RoutedEventArgs e) => Joint3D64.TurnInPlane(90);
		private void Joint64_FlipNormal(object sender, RoutedEventArgs e) => Joint3D64.FlipNormal();

		/// <summary>Double-click a row for the derivation behind its numbers.</summary>
		private void Grid64_ShowDerivation(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (Grid64.SelectedItem is not Joint64RowView view) return;
			if (!view.CanShowDetail)
			{
				ShowStatus(view.SkipReason ?? "nothing was checked for this brace");
				return;
			}

			// The connection and the load effect come from the tab, not from the row: a row knows its
			// governing state only in envelope mode, and neither ever knew which connection it was
			// from. Several of these windows are meant to be open side by side, so each has to name
			// its own joint, state and brace or they cannot be told apart.
			string conName = (Cmb64Connection.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
			string leName = Rb64Envelope.IsChecked == true
				? "envelope"
				: (Cmb64Le.SelectedItem as Le64Option)?.Name ?? "";

			var window = new Controls.Joint64DerivationWindow(view, Owner ?? this, conName, leName);
			window.Show();
		}
	}
}
