using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace YjkInstaller
{
	internal class YjkConfig
	{
		private const string PluginDllName = "IDEAStatiCa.yjk.dll";
		private const string ApiPlugListFileName = "apiPlugList.txt";
		private const string CuiFileName = "YJK.CUI";
		private const string ExeConfigFileName = "yjks.exe.config";
		private const string IdeaMenuMacroUid = "ID_idea_statica";

		private readonly string _installPath;

		// Assembly binding redirects that the plugin requires but may be missing from stock yjks.exe.config
		private static readonly (string Name, string PublicKeyToken, string OldVersion, string NewVersion)[] PluginBindingRedirects =
		{
			("Microsoft.Bcl.AsyncInterfaces",             "cc7b13ffcd2ddd51", "0.0.0.0-10.0.0.0", "8.0.0.0"),
			("System.Runtime.CompilerServices.Unsafe",    "b03f5f7f11d50a3a", "0.0.0.0-6.0.3.0",  "6.0.3.0"),
			("System.Memory",                             "cc7b13ffcd2ddd51", "0.0.0.0-4.0.5.0",  "4.0.5.0"),
			("System.Threading.Tasks.Extensions",         "cc7b13ffcd2ddd51", "0.0.0.0-4.2.4.0",  "4.2.4.0"),
			("System.Collections.Immutable",              "b03f5f7f11d50a3a", "0.0.0.0-5.0.0.0",  "5.0.0.0"),
			("System.Buffers",                            "cc7b13ffcd2ddd51", "0.0.0.0-4.0.3.0",  "4.0.3.0"),
			("System.IO.Pipelines",                       "cc7b13ffcd2ddd51", "0.0.0.0-6.0.0.1",  "6.0.0.1"),
		};

		public YjkConfig(string installPath)
		{
			_installPath = installPath;
		}

		// ── apiPlugList.txt ──────────────────────────────────────────────────────

		public bool IsPluginInApiPlugList()
		{
			string path = Path.Combine(_installPath, ApiPlugListFileName);
			if (!File.Exists(path)) return false;
			return File.ReadAllLines(path).Any(l => l.Trim().Equals(PluginDllName, StringComparison.OrdinalIgnoreCase));
		}

		public void AddToApiPlugList()
		{
			string path = Path.Combine(_installPath, ApiPlugListFileName);
			if (IsPluginInApiPlugList()) return;

			string line = PluginDllName;
			if (File.Exists(path))
			{
				string existing = File.ReadAllText(path);
				string newContent = existing.TrimEnd() + Environment.NewLine + line + Environment.NewLine;
				File.WriteAllText(path, newContent, new UTF8Encoding(false));
			}
			else
			{
				File.WriteAllText(path, line + Environment.NewLine, new UTF8Encoding(false));
			}

			Console.WriteLine($"Added '{PluginDllName}' to {ApiPlugListFileName}.");
		}

		public void RemoveFromApiPlugList()
		{
			string path = Path.Combine(_installPath, ApiPlugListFileName);
			if (!File.Exists(path)) return;

			var lines = File.ReadAllLines(path)
				.Where(l => !l.Trim().Equals(PluginDllName, StringComparison.OrdinalIgnoreCase))
				.ToArray();

			File.WriteAllLines(path, lines, new UTF8Encoding(false));
			Console.WriteLine($"Removed '{PluginDllName}' from {ApiPlugListFileName}.");
		}

		// ── YJK.CUI ──────────────────────────────────────────────────────────────

		public bool IsPluginInCui()
		{
			string path = Path.Combine(_installPath, CuiFileName);
			if (!File.Exists(path)) return false;
			XDocument doc = XDocument.Load(path);
			return doc.Descendants("MenuMacro").Any(e => (string)e.Attribute("UID") == IdeaMenuMacroUid);
		}

		public void AddToCui()
		{
			string path = Path.Combine(_installPath, CuiFileName);
			if (!File.Exists(path))
			{
				Console.WriteLine($"WARNING: {CuiFileName} not found at '{path}'. Skipping CUI update.");
				return;
			}

			XDocument doc = XDocument.Load(path);

			if (doc.Descendants("MenuMacro").Any(e => (string)e.Attribute("UID") == IdeaMenuMacroUid))
			{
				Console.WriteLine($"IDEA StatiCa entry already present in {CuiFileName}.");
				return;
			}

			BackupFile(path);

			// Insertion point 1: add MenuMacro definition to MacroGroup Name="YjkMacros"
			XElement macroGroup = doc.Descendants("MacroGroup")
				.FirstOrDefault(e => (string)e.Attribute("Name") == "YjkMacros");
			if (macroGroup == null)
				Console.WriteLine($"WARNING: <MacroGroup Name=\"YjkMacros\"> not found in {CuiFileName}. Skipping MacroGroup update.");
			else
			{
				XElement menuMacro = new XElement("MenuMacro", new XAttribute("UID", IdeaMenuMacroUid),
					new XElement("Command", "idea_statica"),
					new XElement("HelpString", new XAttribute("UID", "XLS_Column"), "IDEA StatiCa"),
					new XElement("HelpRef", "IDEA StatiCa"),
					new XElement("Description", "IDEA StatiCa"),
					new XElement("RcImage", new XAttribute("Name", "RCDATA_2525")));
				macroGroup.Add(menuMacro);
				Console.WriteLine($"Added MenuMacro to MacroGroup in {CuiFileName}.");
			}

			// Insertion point 2: add SubPanel button to SubRibbon Id="IDModule_Layer" after the "导出IFC" SubPanel
			XElement subRibbon = doc.Descendants("SubRibbon")
				.FirstOrDefault(e => (string)e.Attribute("Id") == "IDModule_Layer");
			if (subRibbon == null)
				Console.WriteLine($"WARNING: <SubRibbon Id=\"IDModule_Layer\"> not found in {CuiFileName}. Skipping SubRibbon update.");
			else
			{
				XElement ifcPanel = subRibbon.Descendants("SubPanel")
					.FirstOrDefault(e => (string)e.Attribute("Title") == "导出IFC");
				if (ifcPanel == null)
					Console.WriteLine($"WARNING: <SubPanel Title=\"导出IFC\"> not found in {CuiFileName}. Skipping SubRibbon update.");
				else
				{
					XElement subPanel = new XElement("SubPanel", new XAttribute("Title", "IDEA StatiCa"),
						new XElement("RibbonRow", new XAttribute("Title", ""),
							new XElement("UIElement",
								new XAttribute("ElementType", "CommandButton"),
								new XAttribute("Label", "IDEA StatiCa"),
								new XAttribute("Orientation", "Vertical"),
								new XAttribute("Id", "ID_idea_statica"),
								new XAttribute("ShowLabel", "True"),
								new XAttribute("ResizeStyle", "Auto"),
								new XAttribute("Size", "Large"),
								new XAttribute("MenuMacroId", "ID_idea_statica"))));
					ifcPanel.AddAfterSelf(subPanel);
					Console.WriteLine($"Added SubPanel to SubRibbon in {CuiFileName}.");
				}
			}

			doc.Save(path);
		}

		public void RemoveFromCui()
		{
			string path = Path.Combine(_installPath, CuiFileName);
			if (!File.Exists(path)) return;

			XDocument doc = XDocument.Load(path);
			bool changed = false;

			// Remove MenuMacro definition
			var menuMacro = doc.Descendants("MenuMacro")
				.FirstOrDefault(e => (string)e.Attribute("UID") == IdeaMenuMacroUid);
			if (menuMacro != null)
			{
				menuMacro.Remove();
				Console.WriteLine($"Removed MenuMacro from {CuiFileName}.");
				changed = true;
			}

			// Remove SubPanel ribbon button
			var subPanel = doc.Descendants("SubPanel")
				.FirstOrDefault(e => (string)e.Attribute("Title") == "IDEA StatiCa");
			if (subPanel != null)
			{
				subPanel.Remove();
				Console.WriteLine($"Removed SubPanel from {CuiFileName}.");
				changed = true;
			}

			if (!changed) return;

			BackupFile(path);
			doc.Save(path);
		}

		// ── yjks.exe.config ──────────────────────────────────────────────────────

		public void MergeBindingRedirects()
		{
			string path = Path.Combine(_installPath, ExeConfigFileName);
			if (!File.Exists(path))
			{
				Console.WriteLine($"WARNING: {ExeConfigFileName} not found at '{path}'. Skipping config update.");
				return;
			}

			XDocument doc = XDocument.Load(path);
			XNamespace ns = "urn:schemas-microsoft-com:asm.v1";

			XElement assemblyBinding = doc.Descendants(ns + "assemblyBinding").FirstOrDefault();
			if (assemblyBinding == null)
			{
				Console.WriteLine($"WARNING: No <assemblyBinding> element found in {ExeConfigFileName}. Skipping.");
				return;
			}

			bool changed = false;
			foreach (var (name, token, oldVer, newVer) in PluginBindingRedirects)
			{
				bool exists = assemblyBinding.Elements(ns + "dependentAssembly").Any(da =>
				{
					var identity = da.Element(ns + "assemblyIdentity");
					return identity != null &&
						(string)identity.Attribute("name") == name &&
						(string)identity.Attribute("publicKeyToken") == token;
				});

				if (exists) continue;

				BackupFile(path);

				XElement newEntry = new XElement(ns + "dependentAssembly",
					new XElement(ns + "assemblyIdentity",
						new XAttribute("name", name),
						new XAttribute("publicKeyToken", token),
						new XAttribute("culture", "neutral")),
					new XElement(ns + "bindingRedirect",
						new XAttribute("oldVersion", oldVer),
						new XAttribute("newVersion", newVer)));

				assemblyBinding.Add(newEntry);
				Console.WriteLine($"Added binding redirect for '{name}' to {ExeConfigFileName}.");
				changed = true;
			}

			if (changed)
				doc.Save(path);
		}

		public void RemoveBindingRedirects()
		{
			string path = Path.Combine(_installPath, ExeConfigFileName);
			if (!File.Exists(path)) return;

			XDocument doc = XDocument.Load(path);
			XNamespace ns = "urn:schemas-microsoft-com:asm.v1";

			XElement assemblyBinding = doc.Descendants(ns + "assemblyBinding").FirstOrDefault();
			if (assemblyBinding == null) return;

			bool changed = false;
			foreach (var (name, token, _, _) in PluginBindingRedirects)
			{
				var entry = assemblyBinding.Elements(ns + "dependentAssembly").FirstOrDefault(da =>
				{
					var identity = da.Element(ns + "assemblyIdentity");
					return identity != null &&
						(string)identity.Attribute("name") == name &&
						(string)identity.Attribute("publicKeyToken") == token;
				});

				if (entry == null) continue;

				if (!changed) BackupFile(path);
				entry.Remove();
				Console.WriteLine($"Removed binding redirect for '{name}' from {ExeConfigFileName}.");
				changed = true;
			}

			if (changed)
				doc.Save(path);
		}

		// ── helpers ──────────────────────────────────────────────────────────────

		private static void BackupFile(string path)
		{
			string bakPath = path + ".bak";
			int i = 1;
			while (File.Exists(bakPath) && i < 100)
			{
				bakPath = path + $".bak{i}";
				i++;
			}

			if (!File.Exists(bakPath))
				File.Copy(path, bakPath);
		}
	}
}
