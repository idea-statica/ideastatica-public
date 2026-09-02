using System.IO;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace NorsokChecker.Services
{
	/// <summary>
	/// One WebView2 user-data folder for the whole app, under %LOCALAPPDATA%.
	///
	/// WebView2 cannot run without a user-data folder (a browser profile: cache, cookies, the
	/// downloaded Edge components), and left to itself it creates "&lt;exe name&gt;.WebView2" NEXT TO
	/// THE EXECUTABLE. That is wrong here for two reasons:
	///
	///   - it is per-EXE, so every build the user keeps grows its own copy of the same 200-300 files
	///     (measured: seven builds side by side, each with its own profile);
	///   - it writes into whatever folder the exe was put in, which may be read-only, a network share
	///     or a download folder — none of which is where per-user state belongs on Windows.
	///
	/// A single fixed path under LocalApplicationData fixes both: one profile, in the place Windows
	/// designates for it, shared by every version of the app and by both WebView2 controls (the
	/// report tab and the derivation window — they must share, or the app pays for two profiles).
	///
	/// Failure is not fatal: if the environment cannot be created, the callers fall back to the plain
	/// EnsureCoreWebView2Async() and WebView2 does what it did before.
	/// </summary>
	internal static class WebViewEnvironment
	{
		private static CoreWebView2Environment? _shared;
		private static bool _failed;

		/// <summary>%LOCALAPPDATA%\IdeaStatiCa\NorsokChecker\WebView2</summary>
		internal static string UserDataFolder => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"IdeaStatiCa", "NorsokChecker", "WebView2");

		/// <summary>
		/// Initialise a WebView2 control against the shared environment. Falls back to the control's
		/// own default if the environment cannot be created, so a profile problem degrades to the old
		/// behaviour rather than leaving the view blank.
		/// </summary>
		internal static async Task EnsureAsync(WebView2 view)
		{
			if (!_failed)
			{
				try
				{
					_shared ??= await CoreWebView2Environment.CreateAsync(
						browserExecutableFolder: null, userDataFolder: UserDataFolder);
					await view.EnsureCoreWebView2Async(_shared);
					return;
				}
				catch (Exception ex)
				{
					// One attempt only: if the folder cannot be used (permissions, a full disk, a
					// locked profile), retrying per view would fail the same way every time.
					_failed = true;

					// Logged, because an uninitialised WebView2 shows nothing and says nothing: this
					// catch was silent, and the fallback below can leave a blank view with no trace
					// of why anywhere.
					AppLog.ReportFailure(
						$"The shared WebView2 profile ({UserDataFolder}) could not be used; "
						+ "falling back to the control's own", ex);
				}
			}

			await view.EnsureCoreWebView2Async();
		}
	}
}
