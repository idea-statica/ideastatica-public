using BimApiLinkFeaExample.Importers;
using FeaExampleApi;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.Plugin;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BimApiLinkFeaExample
{
	/// <summary>
	/// Starts IDEA StatiCa Checkbot linked to the example FEA application. A real link calls <see cref="Run"/> from a
	/// command of the FEA application.
	/// </summary>
	public static class CheckbotLink
	{
		/// <summary>
		/// The name Checkbot shows for the linked application.
		/// </summary>
		private const string ApplicationName = "FEA Example";

		/// <summary>
		/// Opens Checkbot and answers its requests until Checkbot is closed. The Checkbot project is kept in
		/// <paramref name="projectPath"/>, so the next run with the same folder opens it again.
		/// </summary>
		/// <param name="checkbotPath">Full path to IdeaCheckbot.exe of the installed IDEA StatiCa.</param>
		/// <param name="projectPath">
		/// Full path to a writable folder for the Checkbot project, one per FEA model, for example next to the model
		/// file. It is created if it does not exist.
		/// </param>
		/// <param name="feaApi">The FEA application to read the model and its results from.</param>
		/// <param name="logger">Receives the log of the link.</param>
		public static async Task Run(string checkbotPath, string projectPath, IFeaApi feaApi, IPluginLogger logger)
		{
			Directory.CreateDirectory(projectPath);
			logger.LogInformation($"Starting '{checkbotPath}' with the project in '{projectPath}'");

			try
			{
				await FeaBimLink.Create(ApplicationName, projectPath)
					.WithIdeaStatiCa(checkbotPath)
					.WithImporters(importers => RegisterImporters(importers, feaApi))
					.WithResultsImporters(results => results.RegisterImporter(new ResultsImporter(feaApi)))
					.WithLogger(logger)
					.Run(new Model(feaApi.Geometry, feaApi.Loads));
			}
			catch (Exception ex)
			{
				logger.LogError("The FEA example link failed", ex);
				throw;
			}
		}

		/// <summary>
		/// Checkbot asks for every object by its identifier, and BimApiLink passes the request to the importer of that
		/// kind of object. One importer serves all objects of its kind.
		/// </summary>
		private static void RegisterImporters(ImportersConfiguration importers, IFeaApi fea)
		{
			importers
				.RegisterImporter(new CombinationImporter(fea.Loads))
				.RegisterImporter(new CrossSectionImporter(fea.Geometry))
				.RegisterImporter(new LoadCaseImporter(fea.Loads))
				.RegisterImporter(new LoadGroupImporter(fea.Loads))
				.RegisterImporter(new MaterialImporter(fea.Geometry))
				.RegisterImporter(new MemberImporter(fea.Geometry))
				.RegisterImporter(new NodeImporter(fea.Geometry));
		}
	}
}
