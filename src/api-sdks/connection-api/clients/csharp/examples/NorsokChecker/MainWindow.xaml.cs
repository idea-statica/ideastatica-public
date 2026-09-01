using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;
using NorsokChecker.Services;

namespace NorsokChecker
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private readonly ObservableCollection<ConnectionCheckResult> _connections = new();
		private readonly ObservableCollection<MemberDisplayInfo> _members = new();
		private ConnectionApiServiceRunner? _runner;

		/// <summary>
		/// True once we have started a service of our own — the only case in which shutting one down
		/// is ours to do. A service the user was already running must be left alone: taking it down
		/// would remove something this app does not own.
		/// </summary>
		private bool _startedOwnService;

		/// <summary>
		/// Kills our service even if this app is killed outright, which Dispose cannot. Held for the
		/// window's lifetime because closing its handle IS the kill.
		/// </summary>
		private ServiceReaper? _reaper;
		private IConnectionApiClient? _apiClient;
		private Guid _projectId;

		/// <summary>
		/// Cancels the running check. Non-null only while a check is in progress.
		///
		/// The token reaches the API calls themselves: this client version DOES take a
		/// cancellationToken on CalculateAsync and GetRawJsonResultsAsync (verified by compiling a
		/// named-argument call — reflection could not answer it, the assembly's dependencies do not
		/// load standalone, and there is no XML doc beside the DLL). So a stop aborts the in-flight
		/// request rather than only landing between steps.
		///
		/// NOT verified: what the SERVICE does with an aborted request — whether the engine drops
		/// the calculation or finishes it unread. That cannot be read off the method signature.
		/// The ThrowIfCancellationRequested checkpoints below therefore stay: they are what makes a
		/// stop clean for the steps this app runs itself.
		/// </summary>
		private CancellationTokenSource? _checkCts;

		/// <summary>All formula evaluation results, keyed by connection ID.</summary>
		private readonly Dictionary<int, List<NorsokFormulaResult>> _formulaResults = new();

		/// <summary>
		/// The §6.4 topology per connection: every load effect's brace checks, classification and
		/// chord stresses. The results table above holds only the envelope (the governing state per
		/// brace), which cannot answer "show me LE7" or "how was this number reached".
		/// Present even for a rejected joint — its errors are what the tab then lists.
		/// </summary>
		private readonly Dictionary<int, Services.Norsok64.JointTopology> _topologyPerConnection = new();

		/// <summary>
		/// Members per connection, read once when the project is opened. Switching connections then
		/// costs nothing — it used to re-read members and re-export the IOM on every click.
		/// </summary>
		private readonly Dictionary<int, List<MemberDisplayInfo>> _membersPerConnection = new();

		/// <summary>
		/// Drawn member bodies per connection, for the 3D view. Fetched on first selection rather
		/// than up front: the presentation payload is around 1.7 MB per connection.
		/// </summary>
		private readonly Dictionary<int, List<MemberMesh>> _meshesPerConnection = new();

		public event PropertyChangedEventHandler? PropertyChanged;

		public MainWindow()
		{
			InitializeComponent();
			ConnectionsGrid.ItemsSource = _connections;
			MembersGrid.ItemsSource = _members;
			DataContext = this;
			BuildUtilisationLegend();
			HookGroup64Band();
			PrefillServicePath(ServiceRootForTest);
			Log("Norsok Checker ready. Configure API path and load a project.");
		}

		/// <summary>
		/// Put the installation this app would actually use into the path box.
		///
		/// The box used to hold a hardcoded "…\StatiCa 26.0". That was a guess, and the app already
		/// knows better: ServiceLocator resolves the same rule against the registry. On a machine
		/// without 26.0 the hardcoded path pointed at a folder that does not exist, and the app only
		/// recovered by failing the File.Exists test in ResolveSetupDir and searching then — so the
		/// box showed a path that was never going to be used, and the user had no way to see which
		/// version would be.
		///
		/// Preferred version first, otherwise newest — the same order ResolveSetupDir applies, so
		/// what is shown is what will run. The hardcoded value in the XAML stays as the last resort
		/// for a machine where nothing is found: an empty box would say less than a wrong path does.
		///
		/// <param name="rootOverride">
		/// A directory to search instead of the machine's real installs — for tests only. It exists
		/// because on a machine that HAS the preferred version the prefilled value coincides with
		/// the old hardcoded one, so a test could not tell a derived value from a constant: it
		/// compared the path with itself and passed either way (measured 2026-08-27 — the first
		/// version of these tests survived removing the prefill entirely).
		/// </param>
		/// </summary>
		/// <summary>
		/// Set before constructing the window to make it search a synthetic tree instead of the
		/// machine's real installs. Null in the app.
		///
		/// A static rather than a constructor parameter because the window is created by WPF from
		/// XAML, and because the prefill has to be exercised THROUGH the constructor: a test that
		/// calls PrefillServicePath itself cannot notice that the constructor stopped calling it —
		/// measured, that is exactly what the first version of these tests missed.
		/// </summary>
		private void Log(string message)
		{
			Dispatcher.Invoke(() =>
			{
				var timestamp = DateTime.Now.ToString("HH:mm:ss");
				LogBox.AppendText($"[{timestamp}] {message}\n");
				LogBox.ScrollToEnd();
			});
		}

		private void ShowStatus(string text)
		{
			Dispatcher.Invoke(() =>
			{
				StatusText.Text = text;
				StatusBar.Visibility = Visibility.Visible;
				var sb = (System.Windows.Media.Animation.Storyboard)StatusBar.Resources["SpinAnimation"];
				sb.Begin();
			});
		}

		private void HideStatus()
		{
			Dispatcher.Invoke(() =>
			{
				var sb = (System.Windows.Media.Animation.Storyboard)StatusBar.Resources["SpinAnimation"];
				sb.Stop();
				StatusBar.Visibility = Visibility.Collapsed;
			});
		}








		protected override void OnClosed(EventArgs e)
		{
			// Only ours. A service the user was already running is left alone — see
			// _startedOwnService; taking it down would remove something this app does not own.
			if (_startedOwnService)
			{
				try { _runner?.Dispose(); } catch { }
			}
			_runner = null;

			// Closing the job's handle is what kills its processes, so this must run whether or not
			// Dispose above succeeded — it is the mechanism that survives a kill.
			try { _reaper?.Dispose(); } catch { }
			_reaper = null;

			base.OnClosed(e);
		}

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
