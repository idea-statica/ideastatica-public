using System.IO;
using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The service-path box must show the installation that will ACTUALLY be used.
	///
	/// It used to hold a hardcoded "…\StatiCa 26.0" from the XAML. On a machine without 26.0 that
	/// path does not exist, and the app only recovered because ResolveSetupDir's File.Exists test
	/// failed and it searched then — so the box displayed a path that was never going to be used,
	/// and nothing told the user which version would be.
	///
	/// These are STA because the window builds WPF controls, and they drive the real window rather
	/// than re-deciding the rule: a test that restates the precedence cannot fail when it changes.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class ServicePathPrefillTests
	{
		[OneTimeSetUp]
		public void EnsureApplication()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
		}

		/// <summary>
		/// A synthetic install tree whose version is deliberately NOT the preferred one, so the
		/// derived value cannot coincide with the old hardcoded "…\StatiCa 26.0".
		///
		/// This is the whole point. The first version of these tests read the machine's real
		/// installs, and this machine HAS 26.0 — so the prefilled path equalled the hardcoded path
		/// and the assertions compared a value with itself. An oracle run proved it: removing the
		/// prefill entirely left all four tests green.
		/// </summary>
		private static string MakeTree(params string[] folders)
		{
			string root = Path.Combine(Path.GetTempPath(), "norsok-prefill-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(root);   // also covers the no-folders case, which must EXIST but be empty
			foreach (string name in folders)
			{
				string dir = Path.Combine(root, name);
				Directory.CreateDirectory(dir);
				File.WriteAllText(Path.Combine(dir, ServiceLocator.ExeName), "stub");
			}
			return root;
		}

		/// <summary>
		/// THE test: the box is filled BY THE CONSTRUCTOR with what was found.
		///
		/// It has to go through the constructor. A test that calls PrefillServicePath itself passes
		/// even when the constructor no longer calls it — measured 2026-08-27, that is precisely
		/// what the first two versions of this fixture missed, and an oracle run caught it.
		/// </summary>
		[Test]
		public void TheConstructorFillsTheBoxWithWhatWasFound()
		{
			string root = MakeTree("Connection API");
			NorsokChecker.MainWindow.ServiceRootForTest = root;
			try
			{
				var w = new NorsokChecker.MainWindow();   // no explicit prefill call

				Assert.Multiple(() =>
				{
					Assert.That(w.TxtApiPath.Text, Is.EqualTo(Path.Combine(root, "Connection API")),
						"the constructor must prefill from the detected installation");
					Assert.That(File.Exists(Path.Combine(w.TxtApiPath.Text, ServiceLocator.ExeName)),
						Is.True, "and the prefilled path must actually hold the exe");
					Assert.That(w.TxtApiPath.Text, Does.Not.Contain("Program Files"),
						"a hardcoded path would still point into the real Program Files");
				});
			}
			finally
			{
				NorsokChecker.MainWindow.ServiceRootForTest = null;
				try { Directory.Delete(root, true); } catch { }
			}
		}

		/// <summary>
		/// The same, called directly — keeps the method itself covered independently of the wiring.
		///
		/// It reads as a duplicate of the test above and is not one: that test proves the CONSTRUCTOR
		/// calls the prefill, this one proves the prefill picks the right path. Two failure modes,
		/// and the pair exists because the first two versions of this fixture had only the direct
		/// call and stayed green while the constructor had stopped calling it (measured 2026-08-27).
		/// Delete either half and one of the two modes stops being covered.
		/// </summary>
		[Test]
		public void TheBoxShowsWhatWasFoundNotTheHardcodedPath()
		{
			string root = MakeTree("Connection API");
			try
			{
				var w = new NorsokChecker.MainWindow();
				w.PrefillServicePath(root);

				Assert.Multiple(() =>
				{
					Assert.That(w.TxtApiPath.Text, Is.EqualTo(Path.Combine(root, "Connection API")),
						"the only installation in this tree is this one, so that is what must be shown");
					Assert.That(w.TxtApiPath.Text, Does.Not.Contain("Program Files"),
						"a hardcoded path would still point into the real Program Files");
					Assert.That(File.Exists(Path.Combine(w.TxtApiPath.Text, ServiceLocator.ExeName)),
						Is.True, "and the path must actually hold the exe");
				});
			}
			finally { try { Directory.Delete(root, true); } catch { } }
		}

		/// <summary>
		/// With several present, the preferred version wins over the newer ones — the same rule
		/// ResolveSetupDir applies, so the box and the launch agree.
		/// </summary>
		[Test]
		public void ThePreferredVersionWinsOverNewerOnes()
		{
			string root = MakeTree("StatiCa 26.1", "StatiCa 27.0", "StatiCa 26.0");
			try
			{
				var w = new NorsokChecker.MainWindow();
				w.PrefillServicePath(root);

				Assert.That(w.TxtApiPath.Text, Is.EqualTo(Path.Combine(root, "StatiCa 26.0")));
			}
			finally { try { Directory.Delete(root, true); } catch { } }
		}

		/// <summary>
		/// A version below the minimum is not offered even when it is the only one there: /api/4
		/// does not exist before 26.0, so prefilling it would hand the user a path that cannot work.
		/// The box keeps whatever it had, and ResolveSetupDir reports the problem at launch.
		/// </summary>
		[Test]
		public void AnInstallationBelowTheMinimumIsNotPrefilled()
		{
			string root = MakeTree("StatiCa 25.1");
			try
			{
				var w = new NorsokChecker.MainWindow();
				string before = w.TxtApiPath.Text;
				w.PrefillServicePath(root);

				Assert.That(w.TxtApiPath.Text, Is.EqualTo(before),
					"25.1 is below the minimum, so it must not be offered as the path to use");
			}
			finally { try { Directory.Delete(root, true); } catch { } }
		}

		/// <summary>
		/// An empty tree leaves the box alone rather than blanking it — a wrong path says more than
		/// nothing, and ResolveSetupDir names what IS installed when it fails.
		/// </summary>
		[Test]
		public void AnEmptyTreeLeavesTheBoxAlone()
		{
			string root = MakeTree();   // created, but with no installs inside
			try
			{
				var w = new NorsokChecker.MainWindow();
				string before = w.TxtApiPath.Text;
				w.PrefillServicePath(root);

				Assert.That(w.TxtApiPath.Text, Is.EqualTo(before));
			}
			finally { try { Directory.Delete(root, true); } catch { } }
		}

		/// <summary>
		/// Switching to attach and back must leave a FOLDER in the box, never the URL — the label
		/// beside it changes to "IDEA StatiCa folder:" and a URL there would be read as one.
		/// </summary>
		[Test]
		public void SwitchingBackFromAttachRestoresTheDetectedFolder()
		{
			string root = MakeTree("Connection API");
			NorsokChecker.MainWindow.ServiceRootForTest = root;
			try
			{
				var w = new NorsokChecker.MainWindow();

				w.RbAttach.IsChecked = true;
				Assert.That(w.TxtApiPath.Text, Does.StartWith("http"), "attach mode offers a URL");

				w.RbSpawn.IsChecked = true;

				Assert.Multiple(() =>
				{
					Assert.That(w.TxtApiPath.Text, Does.Not.StartWith("http"),
						"back in spawn mode the box means a folder, so a URL must not survive");
					// the DETECTED one, not the hardcoded guess — this is what makes the assertion
					// able to fail when the mode switch writes a constant again
					Assert.That(w.TxtApiPath.Text, Is.EqualTo(Path.Combine(root, "Connection API")));
				});
			}
			finally
			{
				NorsokChecker.MainWindow.ServiceRootForTest = null;
				try { Directory.Delete(root, true); } catch { }
			}
		}

		/// <summary>
		/// The locator's own precedence, stated once here so the intent is recorded next to the UI
		/// test: preferred version first, then newest down. Uses a synthetic tree so it does not
		/// depend on what is installed.
		/// </summary>
		[Test]
		public void ThePrecedenceIsPreferredThenNewest()
		{
			string root = Path.Combine(Path.GetTempPath(), "norsok-prefill-" + Guid.NewGuid().ToString("N"));
			try
			{
				foreach (string name in new[] { "StatiCa 26.1", "StatiCa 26.0", "StatiCa 27.0" })
				{
					string dir = Path.Combine(root, name);
					Directory.CreateDirectory(dir);
					File.WriteAllText(Path.Combine(dir, ServiceLocator.ExeName), "stub");
				}

				var order = ServiceLocator.FindInstalls(root).Select(i => i.Version).ToList();

				Assert.That(order, Is.EqualTo(new[]
				{
					new Version(26, 0),   // preferred, even though 27.0 and 26.1 are newer
					new Version(27, 0),
					new Version(26, 1),
				}));
			}
			finally
			{
				try { Directory.Delete(root, recursive: true); } catch { }
			}
		}
	}
}
