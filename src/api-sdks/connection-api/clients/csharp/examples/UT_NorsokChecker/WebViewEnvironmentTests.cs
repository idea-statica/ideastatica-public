using System.IO;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The WebView2 profile lives under %LOCALAPPDATA%, once, for every version of the app.
	///
	/// Left to itself WebView2 creates "&lt;exe name&gt;.WebView2" next to the executable. Measured on
	/// 2026-08-31: seven builds kept side by side had seven profiles, each with 200-300 files of the
	/// same downloaded Edge components — and it writes into whatever folder the exe was put in, which
	/// may be read-only or a network share.
	/// </summary>
	[TestFixture]
	public class WebViewEnvironmentTests
	{
		/// <summary>
		/// The folder is under LocalApplicationData and does NOT depend on where the exe sits — the
		/// two properties that distinguish the fix from the default behaviour.
		/// </summary>
		[Test]
		public void TheProfileLivesUnderLocalAppDataAndNotBesideTheExe()
		{
			string folder = NorsokChecker.Services.WebViewEnvironment.UserDataFolder;
			string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string exeDir = Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!;

			Assert.Multiple(() =>
			{
				Assert.That(folder, Does.StartWith(localAppData),
					"per-user state belongs under LocalApplicationData");
				Assert.That(folder, Does.Not.StartWith(exeDir),
					"and must not be created beside the executable");
				Assert.That(folder, Does.Contain("NorsokChecker"),
					"named for the app, so it is identifiable in LocalAppData");
			});
		}

		/// <summary>
		/// Both WebView2 controls go through the shared helper, so they share one profile. Asserted on
		/// the SOURCE, because the alternative — constructing two WebView2 controls and comparing
		/// their environments — needs the Edge runtime and a real window, and would skip on any
		/// machine without them. A direct EnsureCoreWebView2Async() call is what regresses this, and
		/// that is a textual fact.
		/// </summary>
		[Test]
		public void NoCallerBypassesTheSharedEnvironment()
		{
			string root = FindAppSource();
			var offenders = Directory
				.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
				.Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
					&& !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
					&& !f.EndsWith("WebViewEnvironment.cs", StringComparison.Ordinal))
				.Where(f => File.ReadAllText(f).Contains(".EnsureCoreWebView2Async("))
				.Select(Path.GetFileName)
				.ToList();

			Assert.That(offenders, Is.Empty,
				"these call WebView2 directly and would create their own profile: "
				+ string.Join(", ", offenders));
		}

		/// <summary>The NorsokChecker source folder, walked up to from the test assembly.</summary>
		private static string FindAppSource()
		{
			var dir = new DirectoryInfo(Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;

			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source from the test output");
			return Path.Combine(dir!.FullName, "NorsokChecker");
		}
	}
}
