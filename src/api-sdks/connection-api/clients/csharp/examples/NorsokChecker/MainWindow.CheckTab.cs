using System.IO;
using System.Windows;
using Microsoft.Win32;
using NorsokChecker.Models;
using NorsokChecker.Services;

namespace NorsokChecker
{
	/// <summary>
	/// The Check tab: choosing what to assess, and showing it. The connection list, the members
	/// grid, the browse/mode handlers, and the 3D preview beside them.
	///
	/// This is everything the user sees BEFORE a run. The run itself is in MainWindow.Run.cs.
	/// </summary>
	public partial class MainWindow
	{
		/// <summary>
		/// One checkbox per registered chapter, in the registry's order.
		///
		/// Built in code so the toggles cannot disagree with the registry: a chapter with no toggle
		/// could never be run, and a toggle for a chapter that no longer exists would be a promise
		/// the app cannot keep. Both were possible while the list lived in the XAML.
		///
		/// The chapter itself is put in Tag, so reading the selection needs no name lookup.
		/// </summary>
		private void BuildChapterToggles()
		{
			ChapterToggles.Children.Clear();
			foreach (var chapter in Services.Chapters.ChapterRegistry.All)
			{
				ChapterToggles.Children.Add(new System.Windows.Controls.CheckBox
				{
					Content = chapter.DisplayName,
					IsChecked = false,          // running a check is the user's decision
					Tag = chapter,
					Margin = new Thickness(0, 0, 12, 0),
					VerticalAlignment = VerticalAlignment.Center,
				});
			}
		}

		/// <summary>The chapters the user ticked, in registry order.</summary>
		private List<Services.Chapters.IChapter> SelectedChapters() =>
			ChapterToggles.Children
				.OfType<System.Windows.Controls.CheckBox>()
				.Where(cb => cb.IsChecked == true)
				.Select(cb => cb.Tag)
				.OfType<Services.Chapters.IChapter>()
				.ToList();

		private void BrowseApiPath_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFolderDialog { Title = "Select IDEA StatiCa installation folder" };
			if (dialog.ShowDialog() == true)
				TxtApiPath.Text = dialog.FolderName;
		}

		/// <summary>
		/// The one text box serves both modes, so it has to say — and hold — the right thing.
		/// Attaching used to send whatever was in the box to ConnectionApiServiceAttacher, and the
		/// box defaults to the installation folder, so "Attach to running service" always failed:
		/// an install path is not a URL.
		/// </summary>
		private void ServiceMode_Changed(object sender, RoutedEventArgs e)
		{
			if (LblApiPath == null || TxtApiPath == null) return;   // fires during XAML init

			bool attach = RbAttach.IsChecked == true;
			LblApiPath.Text = attach ? "Service URL:" : "IDEA StatiCa folder:";
			if (BtnBrowseApiPath != null)
				BtnBrowseApiPath.IsEnabled = !attach;

			string cur = TxtApiPath.Text.Trim();
			bool looksUrl = cur.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
				|| cur.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
			if (attach && !looksUrl)
				TxtApiPath.Text = "http://localhost:5000";
			else if (!attach && looksUrl)
				// switching back to spawn: the detected installation, not a hardcoded guess —
				// PrefillServicePath leaves the XAML default in place when nothing is found
				PrefillServicePath(ServiceRootForTest);
		}

		private void BrowseProject_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				Filter = "IDEA Connection files (*.ideaCon)|*.ideaCon|All files (*.*)|*.*",
				Title = "Select Connection project file"
			};
			if (dialog.ShowDialog() == true)
				TxtProjectFile.Text = dialog.FileName;
		}

		/// <summary>
		/// Connect to the Connection REST API: reuse one that is already running, or start our own.
		///
		/// The SDK's ConnectionApiServiceRunner picks a free port, waits for the heartbeat and
		/// reports a missing exe clearly — but it always starts a NEW service, never notices one
		/// already running, and never looks at the version. Both gaps cost something real:
		///
		///   - a service this app starts holds an IDEA StatiCa LICENCE SEAT for its lifetime, so
		///     starting a second one beside a perfectly good first takes a seat for nothing;
		///   - a setup folder pointing at 25.1 launches a service with no /api/4 at all, and the
		///     failure only appears on the first call as a bare 404.
		///
		/// So: probe the default port first, check the version of whatever answers, and when a new
		/// service has to be started, resolve the folder against what is actually installed.
		/// </summary>

		private async void LoadProject_Click(object sender, RoutedEventArgs e)
		{
			var projectPath = TxtProjectFile.Text.Trim();
			if (string.IsNullOrEmpty(projectPath) || !File.Exists(projectPath))
			{
				MessageBox.Show("Please select a valid .ideaCon project file.", "Invalid File",
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				Telemetry.ProjectLoadClicked();

				BtnLoadProject.IsEnabled = false;
				ShowStatus("Connecting to API...");
				Log("Connecting to Connection API...");

				_apiClient = await CreateApiClientAsync();
				Log("API ready.");

				Log($"Opening project: {Path.GetFileName(projectPath)}");
				var project = await _apiClient.Project.OpenProjectAsync(projectPath);
				_projectId = project.ProjectId;
				Log($"Project opened. ID = {_projectId}");

				// Apply Norsok code factors
				var settingsService = new ProjectSettingsService(_apiClient, Log);
				await settingsService.ApplyNorsokFactorsAsync(_projectId);

				var connections = project.Connections ?? new();
				_connections.Clear();
				_formulaResults.Clear();
				_topologyPerConnection.Clear();
				_meshesPerConnection.Clear();
				Joint3D?.Clear();

				foreach (var con in connections)
				{
					_connections.Add(new ConnectionCheckResult
					{
						Id = con.Id,
						Name = con.Name ?? $"Connection {con.Id}",
						Status = "Loaded",
						MaxUtilization = 0,
						NorsokPass = "-"
					});
				}

				Log($"Found {connections.Count} connection(s).");

				// Every connection's members are read here, once, so switching between them later is
				// instant and silent.
				_members.Clear();
				if (connections.Count > 0)
				{
					await LoadAllConnectionMembersAsync();
					await LoadLoadEffectCountsAsync();
					ConnectionsGrid.SelectedIndex = 0;
					ShowMembersOf(_connections[0]);
					await ShowJoint3DAsync(_connections[0]);
				}

				BtnRunCheck.IsEnabled = true;
				ApiConfigExpander.IsExpanded = false;

				Telemetry.ProjectLoaded(_connections.Count, _members.Count > 0 && _members.All(m => m.IsCHS));
			}
			catch (Exception ex)
			{
				Telemetry.ProjectLoadFailed(ex);
				AppLog.ReportFailure("Opening the project failed", ex);
				Log($"ERROR: {ex.Message}");
				MessageBox.Show(ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				BtnLoadProject.IsEnabled = true;
				HideStatus();
			}
		}

		/// <summary>
		/// Stops the running check — see <see cref="_checkCts"/> for how far the token reaches.
		/// </summary>
		private void CancelCheck_Click(object sender, RoutedEventArgs e)
		{
			if (_checkCts == null || _checkCts.IsCancellationRequested) return;
			Log("Cancel requested.");
			ShowStatus("Cancelling…");
			BtnCancelCheck.IsEnabled = false;   // one press is enough; it cannot be sped up
			_checkCts.Cancel();
		}

		private void ShowMembersOf(ConnectionCheckResult con)
		{
			_members.Clear();
			foreach (var m in _membersPerConnection.GetValueOrDefault(con.Id) ?? new List<MemberDisplayInfo>())
				_members.Add(m);

			// The chord count is a §6.4 condition, so the check reports it per connection in the
			// Status column and one row per condition in Results. Repeating it above the grid only
			// duplicated it — the Role column already shows which member is the chord.
			int chords = _members.Count(m => m.Role == "Chord");
			if (chords != 1)
				Log($"  {con.Name}: {(chords == 0 ? "no continuous member" : $"{chords} continuous members")}"
					+ " — §6.4 needs exactly one");

			MembersGrid.Items.Refresh();
			UpdateTubularState();
		}

		/// <summary>
		/// The members of ONE connection, with each member matched to its own cross-section and its
		/// D/t taken from the connection's own IOM model where that can be read.
		/// </summary>

		private void MainTabs_SelectionChanged(object sender,
			System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (!ReferenceEquals(e.OriginalSource, MainTabs)) return;   // ignore inner grids' events
			if (ConfigCard == null || LogCard == null) return;          // fires during XAML init

			// Check is the first tab; the rest (§6.4, CBFEM, Results, Report) show results only
			bool onCheck = MainTabs.SelectedIndex == 0;
			ConfigCard.Visibility = onCheck ? Visibility.Visible : Visibility.Collapsed;
			LogCard.Visibility = onCheck ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		/// Selecting a connection shows ITS members — from the cache, so the grid swap costs nothing.
		/// The 3D bodies are fetched on first selection and cached the same way.
		/// </summary>
		private async void ConnectionsGrid_SelectionChanged(object sender,
			System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (!IsLoaded) return;
			// this event bubbles up to the tab control, so it must not be mistaken for a tab change
			e.Handled = true;
			if (ConnectionsGrid.SelectedItem is not ConnectionCheckResult con) return;

			ShowMembersOf(con);
			await ShowJoint3DAsync(con);
		}

		/// <summary>
		/// Fill the 3D view with the connection's member bodies, fetching them the first time only.
		/// A failure here never blocks anything: the view is a picture, not a result.
		/// </summary>
		private async Task ShowJoint3DAsync(ConnectionCheckResult con)
		{
			if (Joint3D == null) return;
			// Names on the Check tab too. The joint is turned by dragging here, so the labels are
			// re-projected on every rotation (SizeChanged / the drag handler call RefreshLabels) —
			// without a name a body is identifiable only by clicking it.
			Joint3D.ShowMemberLabels = true;
			Joint3D.Load(await MeshesForAsync(con.Id, con.Name));
		}

		/// <summary>
		/// One connection's member bodies, fetched the first time and cached.
		///
		/// Shared by both views deliberately. The §6.4 tab used to read the cache directly and show
		/// "0 members" for any connection the user had not first selected on the Check tab — the
		/// cache was only ever filled from there. Worse than an empty picture: the tables beside it
		/// still held the PREVIOUS connection's numbers, so the sheet showed one joint's forces
		/// under another joint's name.
		/// </summary>
		private async Task<List<MemberMesh>> MeshesForAsync(int connectionId, string? connectionName = null)
		{
			if (_apiClient == null || _projectId == Guid.Empty) return new List<MemberMesh>();
			if (_meshesPerConnection.TryGetValue(connectionId, out var cached)) return cached;

			List<MemberMesh> meshes;
			try
			{
				// presentations/text is the app's own drawing of the joint — see
				// JointPresentationReader for the payload's shape
				string json = await _apiClient.Presentation.GetDataScene3DTextAsync(_projectId, connectionId);

				// The connection's real member ids, so a body tagged "member" that is not one of them
				// is drawn as context rather than counted as a member. Measured on test_cs CON15: the
				// payload returns nine "member" groups where /members reports six. Null when the
				// members have not been read yet, which leaves the tag taken at face value.
				var knownIds = _membersPerConnection.TryGetValue(connectionId, out var known) && known.Count > 0
					? known.Select(m => m.Id).ToHashSet()
					: null;

				meshes = JointPresentationReader.ReadMembers(json, Log, knownIds);
			}
			catch (Exception ex)
			{
				Log($"  3D view unavailable for {connectionName ?? $"connection {connectionId}"}: {ex.Message}");
				meshes = new List<MemberMesh>();
			}
			_meshesPerConnection[connectionId] = meshes;
			return meshes;
		}

		/// <summary>Hovering a member row highlights its body in the 3D view.</summary>
		private void MembersGrid_MemberHighlight(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (Joint3D == null) return;
			if (sender is System.Windows.Controls.DataGridRow { Item: MemberDisplayInfo m })
				Joint3D.HighlightMember(m.Id);
		}

		/// <summary>
		/// Leaving a row falls back to the SELECTED member rather than clearing outright — otherwise
		/// picking a member in the 3D view lost its highlight the moment the pointer left the table.
		/// </summary>
		private void MembersGrid_ClearHighlight(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (Joint3D == null) return;
			Joint3D.HighlightMember(MembersGrid.SelectedItem is MemberDisplayInfo sel ? sel.Id : -1);
		}

		/// <summary>
		/// Clicking a body in the 3D view selects its row — the reverse of the hover highlight, so
		/// the two views agree whichever one the user points at. A click on nothing clears both.
		/// </summary>
		private void Joint3D_MemberClicked(object? sender, int memberId)
		{
			if (memberId < 0)
			{
				MembersGrid.SelectedItem = null;
				Joint3D?.HighlightMember(-1);
				return;
			}

			var row = MembersGrid.ItemsSource?.OfType<MemberDisplayInfo>()
				.FirstOrDefault(m => m.Id == memberId);
			if (row == null) return;      // a body with no row (a weld, say) — leave the table alone

			MembersGrid.SelectedItem = row;
			MembersGrid.ScrollIntoView(row);
			Joint3D?.HighlightMember(memberId);
		}

		/// <summary>
		/// Replace each tubular member's D/T with the values measured from the connection's own IOM
		/// model. Port of extract.py enrich_sections_from_iom.
		///
		/// The section map this overrides is built per project from the cross-section name, which is
		/// wrong for most catalogue profiles and can be confidently wrong (PIPE127STD is D = 141.3,
		/// not 127). The IOM facet ring is the modelled geometry, so it is what the check should
		/// stand on; the name survives only as a cross-check in the log.
		///
		/// Never fatal: if the export or a beam cannot be read, the member keeps whatever the name
		/// gave it and the existing gates still decide whether that is good enough.
		/// </summary>

		private void UpdateTubularState()
		{
			int chsCount = _members.Count(m => m.IsCHS);
			int other = _members.Count - chsCount;

			if (_members.Count > 0 && other == 0)
				Log($"  all {_members.Count} members are tubular");
			else if (chsCount > 0)
				Log($"  {chsCount} tubular + {other} other section(s) — §6.4 needs every member tubular");
			else
				Log("  no tubular members — §6.4 does not apply");
		}

		private bool ValidateGeometryInputs()
		{
			foreach (var m in _members.Where(m => m.IsCHS))
			{
				if (m.WallThickness >= m.Diameter / 2)
					Log($"WARNING: {m.Name} t={m.WallThickness}mm must be < D/2={m.Diameter / 2}mm");
				if (m.Diameter / m.WallThickness >= 120)
					Log($"WARNING: {m.Name} D/t={m.Diameter / m.WallThickness:F0} exceeds limit of 120 (§6.3.1)");
			}
			var chord = _members.FirstOrDefault(m => m.Role == "Chord" && m.IsCHS);
			var brace = _members.FirstOrDefault(m => m.Role == "Brace" && m.IsCHS);
			if (chord != null && brace != null && brace.Diameter > chord.Diameter)
				Log($"WARNING: Brace d={brace.Diameter}mm cannot exceed chord D={chord.Diameter}mm");
			// θ is no longer entered by hand — it is derived per brace from the member geometry, and
			// the topology gates report it per brace (θ < 5° an error, outside 30–90° a warning).
			return true;
		}

	}
}
