using System;

namespace IdeaStatiCa.Plugin
{
	[Obsolete("This class will be replaced by IdeaConnectionController controller.")]
	public class IdeaConnectionControllerGrpc : IdeaConnectionController
	{
		private IdeaConnectionControllerGrpc(string ideaInstallDir, IPluginLogger logger) : base(ideaInstallDir, logger)
		{
		}

		/// <summary>
		/// Creates connection and starts IDEA StatiCa connection application.
		/// Call OpenProject after this method to open specific project.
		/// </summary>
		/// <param name="ideaInstallDir">IDEA StatiCa installation directory.</param>
		/// <param name="logger">The logger.</param>
		/// <returns>A controller object.</returns>
		public new static IConnectionController Create(string ideaInstallDir, IPluginLogger logger)
		{
			IdeaConnectionControllerGrpc connectionController = new IdeaConnectionControllerGrpc(ideaInstallDir, logger);
			connectionController.OpenConnectionClient();
			return connectionController;
		}
	}
}
