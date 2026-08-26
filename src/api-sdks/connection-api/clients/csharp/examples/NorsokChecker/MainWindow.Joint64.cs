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
			RefreshJoint64();
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
			ShowJoint64Table(topo, envelope);
			ShowJoint64Plane(topo);
		}

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
				Lbl64VerdictTitle.Text = "✗ No check performed — this joint is outside the scope of NORSOK §6.4";
				Lbl64VerdictBody.Text =
					"Section 6.4 covers simple tubular joints. The joint plane, the chord stresses "
					+ "averaged across it and the K/Y/X force balance are properties of the WHOLE "
					+ "joint, so while any of the conditions below is unmet no brace can be assessed "
					+ "— not even one whose own geometry is fine.";
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

				if (cls != null)
				{
					view.FrK = $"{cls.FrK * 100:F0} %";
					view.FrX = $"{cls.FrX * 100:F0} %";
					view.FrY = $"{cls.FrY * 100:F0} %";
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
				view.Verdict = row.Passed ? "PASS" : "FAIL";

				// ⚠ geometry outside 6.4.3.1 (resistance extrapolated); ⛔ the chord wall has no
				// capacity left at this footprint, which forces FAIL whatever the number says
				string flags = "";
				if (!row.WithinRange) flags += "⚠";
				if (row.ChordOverstressed) flags += "⛔";
				view.Flags = flags;

				rows.Add(view);
			}

			Grid64.ItemsSource = rows;

			int assessed = rows.Count(r => r.Verdict is "PASS" or "FAIL");
			int failed = rows.Count(r => r.Verdict == "FAIL");
			string where = envelope
				? $"envelope over {topo.JointChecks.Count} load effect(s)"
				: (Cmb64Le.SelectedItem as Le64Option)?.Name ?? "—";
			Lbl64Summary.Text = $"{rows.Count} brace(s) · {assessed} assessed · {failed} failed · {where}";

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
		private void ShowJoint64Plane(JointTopology topo)
		{
			if (Cmb64Connection.SelectedItem is not ComboBoxItem { Tag: int conId }) return;
			if (!_meshesPerConnection.TryGetValue(conId, out var meshes))
			{
				Joint3D64.Clear();
				return;
			}

			Joint3D64.Load(meshes);
			var n = topo.NPlane;
			var ex = topo.Ex;
			if (n.Norm > 1e-9)
				Joint3D64.LookAtPlane(new Vector3D(n.X, n.Y, n.Z), new Vector3D(ex.X, ex.Y, ex.Z));
		}

		/// <summary>Hovering a brace row highlights that member in the joint view.</summary>
		private void Grid64_HighlightBrace(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (sender is not DataGridRow { Item: Joint64RowView view }) return;
			var topo = SelectedJoint64Topology();
			var brace = topo?.GapBraces.FirstOrDefault(b => b.Name == view.Brace);
			Joint3D64.HighlightMember(brace?.Id ?? -1);
		}

		/// <summary>Double-click a row for the derivation behind its numbers.</summary>
		private void Grid64_ShowDerivation(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (Grid64.SelectedItem is not Joint64RowView view) return;
			if (!view.CanShowDetail)
			{
				ShowStatus(view.SkipReason ?? "nothing was checked for this brace");
				return;
			}

			var window = new Controls.Joint64DerivationWindow(view, Owner ?? this);
			window.Show();
		}
	}
}
