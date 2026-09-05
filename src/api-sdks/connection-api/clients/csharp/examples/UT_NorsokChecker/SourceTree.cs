using System.IO;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Locating the app's source from the test output — for the tests that read the source because
	/// the thing they guard has no seam to call (a click handler, a XAML attribute, a call site
	/// that must not be bypassed).
	///
	/// FAIL, NOT IGNORE, when the tree is absent. Ten files had grown their own copy of this walk
	/// and every one of them ended in `Assert.Ignore`, so in any binaries-only layout — a CI
	/// artefact, another machine — all ten passed while enforcing nothing. They guard precisely the
	/// "removed a line and every test stayed green" class of defect, so a silent skip is the one
	/// outcome they must not have.
	///
	/// `MainWindowStructureTests.AppDir()` already had the Assert.Fail form, with the reasoning
	/// written down. It was never carried across; this is that fix, in one place, so the next file
	/// cannot inherit the wrong version.
	/// </summary>
	internal static class SourceTree
	{
		/// <summary>The NorsokChecker project directory. Fails the test if it cannot be found.</summary>
		internal static string AppDir()
		{
			var dir = new DirectoryInfo(Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;

			if (dir == null)
				Assert.Fail("cannot locate the NorsokChecker source from the test output — this test "
					+ "reads the source, and skipping it would report a pass it did not earn");

			return Path.Combine(dir!.FullName, "NorsokChecker");
		}

		/// <summary>One source file's text, by path relative to the project directory.</summary>
		internal static string Read(string relativePath) =>
			File.ReadAllText(Path.Combine(AppDir(), relativePath));
	}
}
