using Microsoft.Extensions.Logging;
using System.Text;
using System.Xml.Linq;

namespace DotnetBuildTools
{
	internal class NugetUpdater
	{
		readonly string RepositoryPath;
		readonly ILogger<NugetUpdater> Logger;

		internal NugetUpdater(ILogger<NugetUpdater>? logger, string? repositoryPath)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("NugetUpdater : logger can not be null");
			}

			this.Logger = logger;

			if (string.IsNullOrEmpty(repositoryPath))
			{
				throw new ArgumentNullException("repositoryPath is null");
			}

			if (!Directory.Exists(repositoryPath))
			{
				throw new ArgumentException($"repositoryPath '{repositoryPath}' doesn't exist");
			}

			this.RepositoryPath = repositoryPath;
		}
		internal string GetCurrentVersion(string packageId = "IdeaStatiCa.BimApiLink", string projectFileName = "BimApiLinkFeaExample.csproj")
		{
			Logger.LogDebug($"NugetUpdater.GetCurrentVersion projectFileName = '{projectFileName}' nugetId = '{packageId}'");

			var projectFullFilename = Directory.EnumerateFiles(RepositoryPath, projectFileName, SearchOption.AllDirectories).FirstOrDefault();
			if(string.IsNullOrEmpty(projectFullFilename))
			{
				throw new ArgumentException($"NugetUpdater.GetCurrentVersion failed: project '{projectFileName}' was not found in '{RepositoryPath}'");
			}
			else
			{
				Logger.LogDebug($"NugetUpdater.GetCurrentVersion project '{projectFileName}' was found '{projectFullFilename}'");
			}

			XDocument xdoc = XDocument.Load(projectFullFilename);
			var descendants = xdoc.Descendants();
			var packageReferences = descendants.Where(d =>
			{
				if (d?.Name == null)
				{
					return false;
				}

				string elName = d.Name.ToString();
				return !string.IsNullOrEmpty(elName) && elName.Equals("PackageReference", StringComparison.InvariantCultureIgnoreCase);
			});


			string version = string.Empty;
			var ideaReference = packageReferences.FirstOrDefault(r => {
				var includeAtt = r.Attribute("Include");
				if(includeAtt == null)
				{
					return false;
				}

				if(includeAtt?.Value?.Equals(packageId) == true)
				{
					var versionAtt = r.Attribute("Version");
					if(versionAtt == null || string.IsNullOrEmpty(versionAtt.Value))
					{
						throw new ArgumentException($"NugetUpdater.GetCurrentVersion : Missing the attribute 'Version'");
					}
					version = versionAtt.Value;
					return true;
				}

				return false;
			});

			Logger.LogInformation($"NugetUpdater.GetCurrentVersion : the version of '{packageId}' in the project '{projectFullFilename}' is '{version}'");
			return version;
		}

		internal bool UpdateNugetInCsproj(string currentVersion, string newVersion)
		{
			Logger.LogDebug($"NugetUpdater.Update : currentVersion = '{currentVersion}' newVersion = '{newVersion}'");
			var csProjFiles = Directory.EnumerateFiles(RepositoryPath, "*.csproj", SearchOption.AllDirectories);
			bool isChange = false;
			foreach(var csprojFile in csProjFiles )
			{
				Logger.LogDebug($"NugetUpdater.Update : opening file '{csprojFile}'");
				StringBuilder existing_csproj = new StringBuilder();
				using (TextReader reader = new StreamReader(csprojFile))
				{
					existing_csproj = new StringBuilder(reader.ReadToEnd());
				}

				string existing = existing_csproj.ToString();
				var updated_csproj = existing_csproj.Replace(currentVersion, newVersion);
				string updated = updated_csproj.ToString();
				
				if(updated.Equals(existing))
				{
					Logger.LogInformation($"NugetUpdater.Update : '{csprojFile}' has not been changed");
				}
				else
				{
					isChange = true;
					using (TextWriter writer = new StreamWriter(csprojFile, false))
					{
						writer.Write(updated);
					}

					Logger.LogInformation($"NugetUpdater.Update : '{csprojFile}' has been updated");
				}
			}
			return isChange;
		}
	}
}
