using BimApiLinkCadExample.Importers;
using CadExampleApi;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.Plugin;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BimApiLinkCadExample
{
	/// <summary>
	/// Starts IDEA StatiCa Checkbot linked to the example CAD application. A real link calls <see cref="Run"/> from a
	/// command of the CAD application.
	/// </summary>
	public static class CheckbotLink
	{
		/// <summary>
		/// The name Checkbot shows for the linked application, for example while it waits for the user's selection.
		/// </summary>
		private const string ApplicationName = "CAD Example";

		/// <summary>
		/// Opens Checkbot and answers its requests until Checkbot is closed. The Checkbot project is kept in
		/// <paramref name="projectPath"/>, so the next run with the same folder opens it again.
		/// </summary>
		/// <param name="checkbotPath">Full path to IdeaCheckbot.exe of the installed IDEA StatiCa.</param>
		/// <param name="projectPath">
		/// Full path to a writable folder for the Checkbot project, one per CAD model, for example next to the model
		/// file. It is created if it does not exist.
		/// </param>
		/// <param name="cadApi">The CAD application to read the model from.</param>
		/// <param name="logger">Receives the log of the link.</param>
		public static async Task Run(string checkbotPath, string projectPath, ICadApi cadApi, IPluginLogger logger)
		{
			Directory.CreateDirectory(projectPath);
			logger.LogInformation($"Starting '{checkbotPath}' with the project in '{projectPath}'");

			try
			{
				await CadBimLink.Create(ApplicationName, projectPath)
					.WithIdeaStatiCa(checkbotPath)
					.WithImporters(importers => RegisterImporters(importers, cadApi.Geometry))
					.WithLogger(logger)
					.Run(new Model(cadApi.Geometry));
			}
			catch (Exception ex)
			{
				logger.LogError("The CAD example link failed", ex);
				throw;
			}
		}

		/// <summary>
		/// Checkbot asks for every object by its identifier, and BimApiLink passes the request to the importer of that
		/// kind of object. One importer serves all objects of its kind.
		/// </summary>
		private static void RegisterImporters(ImportersConfiguration importers, ICadGeometryApi cad)
		{
			importers
				.RegisterImporter(new BoltGridImporter(cad))
				.RegisterImporter(new ConnectedMemberImporter())
				.RegisterImporter(new ConnectionImporter())
				.RegisterImporter(new CrossSectionImporter(cad))
				.RegisterImporter(new CutImporter(cad))
				.RegisterImporter(new MaterialImporter(cad))
				.RegisterImporter(new MemberImporter(cad))
				.RegisterImporter(new NegativePlateImporter(cad))
				.RegisterImporter(new NodeImporter())
				.RegisterImporter(new PlateImporter(cad));
		}
	}
}
